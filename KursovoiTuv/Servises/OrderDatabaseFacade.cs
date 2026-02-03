using System;
using System.Collections.Generic;
using System.Linq;
using KursovoiTuv.Data;
using KursovoiTuv.Models;
using KursovoiTuv.Patterns.Facade;
using Microsoft.EntityFrameworkCore;

namespace KursovoiTuv.Services
{
	/// <summary>
	/// Реализация фасада для работы с БД заказов
	/// </summary>
	/// <remarks>
	/// Инкапсулирует всю сложность работы с Entity Framework Core
	/// и предоставляет простой интерфейс для работы с заказами
	/// </remarks>
	public class OrderDatabaseFacade : IOrderDatabaseFacade
	{
		private readonly ApplicationDbContext _context;

		/// <summary>
		/// Конструктор фасада
		/// </summary>
		/// <param name="context">Контекст базы данных</param>
		public OrderDatabaseFacade(ApplicationDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public List<Order> GetAllOrders()
		{
			try
			{
				return _context.Orders
					.OrderByDescending(o => o.Id)
					.ToList();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Ошибка получения заказов: {ex.Message}");
				return new List<Order>();
			}
		}

		public Order GetOrderById(int id)
		{
			try
			{
				return _context.Orders.Find(id);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Ошибка получения заказа: {ex.Message}");
				return null;
			}
		}

		public int AddOrder(Order order)
		{
			try
			{
				if (string.IsNullOrEmpty(order.OrderNumber))
				{
					order.OrderNumber = GenerateOrderNumber();
				}

				_context.Orders.Add(order);
				_context.SaveChanges();
				return order.Id;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Ошибка добавления заказа: {ex.Message}");
				throw;
			}
		}

		public bool UpdateOrder(Order order)
		{
			try
			{
				var existingOrder = _context.Orders.Find(order.Id);
				if (existingOrder == null)
					return false;

				// Копируем все свойства
				_context.Entry(existingOrder).CurrentValues.SetValues(order);
				_context.SaveChanges();
				return true;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Ошибка обновления заказа: {ex.Message}");
				throw;
			}
		}

		public bool DeleteOrder(int id)
		{
			try
			{
				var order = _context.Orders.Find(id);
				if (order == null)
					return false;

				_context.Orders.Remove(order);
				_context.SaveChanges();
				return true;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Ошибка удаления заказа: {ex.Message}");
				throw;
			}
		}

		public List<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
		{
			try
			{
				return _context.Orders
					.Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
					.OrderByDescending(o => o.OrderDate)
					.ToList();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Ошибка получения заказов по дате: {ex.Message}");
				return new List<Order>();
			}
		}

		public List<Order> GetOrdersByStatus(OrderStatus status)
		{
			try
			{
				return _context.Orders
					.Where(o => o.Status == status)
					.OrderByDescending(o => o.OrderDate)
					.ToList();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Ошибка получения заказов по статусу: {ex.Message}");
				return new List<Order>();
			}
		}

		public decimal GetTotalRevenue(DateTime startDate, DateTime endDate)
		{
			try
			{
				return _context.Orders
					.Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
					.Sum(o => o.Cost);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Ошибка расчета выручки: {ex.Message}");
				return 0;
			}
		}

		public int GetOrdersCountByPriority(string priority)
		{
			try
			{
				return _context.Orders
					.Count(o => o.Priority == priority);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Ошибка подсчета заказов по приоритету: {ex.Message}");
				return 0;
			}
		}

		public List<Order> GetOverdueOrders()
		{
			try
			{
				return _context.Orders
					.Where(o => o.Deadline < DateTime.Now &&
							   o.Status != OrderStatus.Completed &&
							   o.Status != OrderStatus.Shipped)
					.OrderBy(o => o.Deadline)
					.ToList();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Ошибка получения просроченных заказов: {ex.Message}");
				return new List<Order>();
			}
		}

		private string GenerateOrderNumber()
		{
			var today = DateTime.Now.ToString("yyyyMMdd");
			var random = new Random().Next(100, 999);
			return $"MP-{today}-{random}";
		}
	}
}