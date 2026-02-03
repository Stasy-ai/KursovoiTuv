using System;
using System.Collections.Generic;
using KursovoiTuv.Models;

namespace KursovoiTuv.Services
{
	/// <summary>
	/// Интерфейс сервиса для работы с заказами
	/// </summary>
	public interface IOrderService
	{
		List<Order> GetAllOrders();
		Order GetOrderById(int id);
		int CreateOrder(Order order);
		bool UpdateOrder(Order order);
		bool DeleteOrder(int id);
		List<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate);
		OrderStatistics GetOrderStatistics(DateTime startDate, DateTime endDate);
		decimal CalculateOrderCost(int quantity, string priority);
	}
}