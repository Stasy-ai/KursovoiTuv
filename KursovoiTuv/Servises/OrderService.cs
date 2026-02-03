using KursovoiTuv.Data;
using KursovoiTuv.Models;
using KursovoiTuv.Patterns.Facade;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KursovoiTuv.Services
{
	/// <summary>
	/// Сервис для работы с заказами
	/// </summary>
	/// <remarks>
	/// Содержит бизнес-логику работы с заказами.
	/// Использует фасад базы данных для доступа к хранилищу.
	/// </remarks>
	public class OrderService : IOrderService
	{
		private readonly IOrderDatabaseFacade _databaseFacade;

		/// <summary>
		/// Конструктор сервиса заказов
		/// </summary>
		/// <param name="databaseFacade">Фасад для работы с БД</param>
		public OrderService(IOrderDatabaseFacade databaseFacade)
		{
			_databaseFacade = databaseFacade ?? throw new ArgumentNullException(nameof(databaseFacade));
		}
		public OrderService()
		{
			// Создаем зависимости вручную
			var dbContext = new ApplicationDbContext();
			_databaseFacade = new OrderDatabaseFacade(dbContext);
		}

		/// <summary>
		/// Получить все заказы
		/// </summary>
		public List<Order> GetAllOrders()
		{
			return _databaseFacade.GetAllOrders();
		}

		/// <summary>
		/// Получить заказ по идентификатору
		/// </summary>
		public Order GetOrderById(int id)
		{
			return _databaseFacade.GetOrderById(id);
		}

		/// <summary>
		/// Создать новый заказ
		/// </summary>
		public int CreateOrder(Order order)
		{
			ValidateOrder(order);
			return _databaseFacade.AddOrder(order);
		}

		/// <summary>
		/// Обновить существующий заказ
		/// </summary>
		public bool UpdateOrder(Order order)
		{
			ValidateOrder(order);
			return _databaseFacade.UpdateOrder(order);
		}

		/// <summary>
		/// Удалить заказ
		/// </summary>
		public bool DeleteOrder(int id)
		{
			return _databaseFacade.DeleteOrder(id);
		}

		/// <summary>
		/// Получить заказы за период
		/// </summary>
		public List<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
		{
			return _databaseFacade.GetOrdersByDateRange(startDate, endDate);
		}

		/// <summary>
		/// Получить статистику по заказам
		/// </summary>
		public OrderStatistics GetOrderStatistics(DateTime startDate, DateTime endDate)
		{
			var orders = _databaseFacade.GetOrdersByDateRange(startDate, endDate);

			return new OrderStatistics
			{
				TotalOrders = orders.Count,
				TotalRevenue = orders.Sum(o => o.Cost),
				AverageOrderValue = orders.Any() ? orders.Average(o => o.Cost) : 0,
				PendingOrders = orders.Count(o => o.Status == OrderStatus.New ||
												  o.Status == OrderStatus.InProgress),
				CompletedOrders = orders.Count(o => o.Status == OrderStatus.Completed ||
													o.Status == OrderStatus.Shipped)
			};
		}

		/// <summary>
		/// Рассчитать стоимость заказа
		/// </summary>
		public decimal CalculateOrderCost(int quantity, string priority)
		{
			const decimal baseCost = 1500m; // Базовая стоимость за единицу

			// КОРРЕКТНЫЕ коэффициенты:
			decimal multiplier = priority switch
			{
				"Высокий" => 1.8m,    // 1.8 * 1500 = 2700
				"Низкий" => 1.2m,     // 1.2 * 1500 = 1800
				"Средний" => 1.5m,    // 1.5 * 1500 = 2250
				_ => 1.5m             // По умолчанию средний
			};

			return quantity * baseCost * multiplier;
		}

		/// <summary>
		/// Валидация заказа
		/// </summary>
		private void ValidateOrder(Order order)
		{
			if (string.IsNullOrWhiteSpace(order.ClientName))
				throw new ArgumentException("Имя клиента обязательно");

			if (string.IsNullOrWhiteSpace(order.ProductDescription))
				throw new ArgumentException("Описание изделия обязательно");

			if (order.Quantity <= 0)
				throw new ArgumentException("Количество должно быть положительным");

			if (order.Deadline < order.OrderDate)
				throw new ArgumentException("Срок выполнения не может быть раньше даты заказа");
		}
	}

	/// <summary>
	/// Статистика по заказам
	/// </summary>
	public class OrderStatistics
	{
		/// <summary>
		/// Общее количество заказов
		/// </summary>
		public int TotalOrders { get; set; }

		/// <summary>
		/// Общая выручка
		/// </summary>
		public decimal TotalRevenue { get; set; }

		/// <summary>
		/// Средняя стоимость заказа
		/// </summary>
		public decimal AverageOrderValue { get; set; }

		/// <summary>
		/// Количество заказов в работе
		/// </summary>
		public int PendingOrders { get; set; }

		/// <summary>
		/// Количество завершенных заказов
		/// </summary>
		public int CompletedOrders { get; set; }
	}
}