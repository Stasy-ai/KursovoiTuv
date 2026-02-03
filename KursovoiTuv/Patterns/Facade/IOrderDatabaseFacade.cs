using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using KursovoiTuv.Models;

namespace KursovoiTuv.Patterns.Facade
{
	/// <summary>
	/// Фасад для работы с базой данных заказов (паттерн Facade)
	/// </summary>
	/// <remarks>
	/// Предоставляет упрощенный интерфейс для сложной системы работы с БД.
	/// Скрывает детали реализации Entity Framework.
	/// </remarks>
	public interface IOrderDatabaseFacade
	{
		/// <summary>
		/// Получить все заказы
		/// </summary>
		/// <returns>Список всех заказов</returns>
		List<Order> GetAllOrders();

		/// <summary>
		/// Получить заказ по идентификатору
		/// </summary>
		/// <param name="id">Идентификатор заказа</param>
		/// <returns>Найденный заказ или null</returns>
		Order GetOrderById(int id);

		/// <summary>
		/// Добавить новый заказ
		/// </summary>
		/// <param name="order">Заказ для добавления</param>
		/// <returns>Идентификатор созданного заказа</returns>
		int AddOrder(Order order);

		/// <summary>
		/// Обновить существующий заказ
		/// </summary>
		/// <param name="order">Заказ с обновленными данными</param>
		/// <returns>true если обновление успешно</returns>
		bool UpdateOrder(Order order);

		/// <summary>
		/// Удалить заказ
		/// </summary>
		/// <param name="id">Идентификатор заказа для удаления</param>
		/// <returns>true если удаление успешно</returns>
		bool DeleteOrder(int id);

		/// <summary>
		/// Получить заказы за указанный период
		/// </summary>
		/// <param name="startDate">Начальная дата</param>
		/// <param name="endDate">Конечная дата</param>
		/// <returns>Список заказов за период</returns>
		List<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate);

		/// <summary>
		/// Получить заказы по статусу
		/// </summary>
		/// <param name="status">Статус заказа</param>
		/// <returns>Список заказов с указанным статусом</returns>
		List<Order> GetOrdersByStatus(OrderStatus status);

		/// <summary>
		/// Получить общую выручку за период
		/// </summary>
		/// <param name="startDate">Начальная дата</param>
		/// <param name="endDate">Конечная дата</param>
		/// <returns>Общая сумма выручки</returns>
		decimal GetTotalRevenue(DateTime startDate, DateTime endDate);

		/// <summary>
		/// Получить количество заказов по приоритету
		/// </summary>
		/// <param name="priority">Приоритет заказа</param>
		/// <returns>Количество заказов</returns>
		int GetOrdersCountByPriority(string priority);

		/// <summary>
		/// Получить просроченные заказы
		/// </summary>
		/// <returns>Список просроченных заказов</returns>
		List<Order> GetOverdueOrders();
	}
}
