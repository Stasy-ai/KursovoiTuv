using KursovoiTuv.Data;
using KursovoiTuv.Models;
using KursovoiTuv.Patterns.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KursovoiTuv.Services
{
	/// <summary>
	/// Сервис для генерации отчетов
	/// </summary>
	/// <remarks>
	/// Содержит логику формирования различных типов отчетов
	/// на основе данных о заказах
	/// </remarks>
	public class ReportService : IReportService
	{
		private readonly IOrderDatabaseFacade _databaseFacade;

		/// <summary>
		/// Конструктор сервиса отчетов
		/// </summary>
		/// <param name="databaseFacade">Фасад для работы с БД</param>
		public ReportService(IOrderDatabaseFacade databaseFacade)
		{
			_databaseFacade = databaseFacade ?? throw new ArgumentNullException(nameof(databaseFacade));
		}

		public ReportService()
		{
			var dbContext = new ApplicationDbContext();
			_databaseFacade = new OrderDatabaseFacade(dbContext);
		}

		/// <summary>
		/// Сгенерировать отчет указанного типа
		/// </summary>
		/// <param name="reportType">Тип отчета</param>
		/// <param name="startDate">Начальная дата</param>
		/// <param name="endDate">Конечная дата</param>
		/// <returns>Текстовый отчет</returns>
		public string GenerateReport(string reportType, DateTime startDate, DateTime endDate)
		{
			var orders = _databaseFacade.GetOrdersByDateRange(startDate, endDate);

			return reportType switch
			{
				"Все заказы" => GenerateAllOrdersReport(orders, startDate, endDate),
				"Заказы по статусу" => GenerateStatusReport(orders, startDate, endDate),
				"Заказы по приоритету" => GeneratePriorityReport(orders, startDate, endDate),
				"Просроченные заказы" => GenerateOverdueReport(startDate, endDate),
				"Финансовый отчет" => GenerateFinancialReport(orders, startDate, endDate),
				"Активные заказы" => GenerateActiveOrdersReport(orders, startDate, endDate),
				_ => "Неизвестный тип отчета"
			};
		}

		/// <summary>
		/// Сгенерировать отчет по всем заказам
		/// </summary>
		private string GenerateAllOrdersReport(List<Order> orders, DateTime startDate, DateTime endDate)
		{
			var report = new StringBuilder();
			report.AppendLine("ОТЧЕТ: Все заказы");
			report.AppendLine($"Период: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}");
			report.AppendLine($"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm}");
			report.AppendLine($"Всего заказов: {orders.Count}");
			report.AppendLine($"Общая стоимость: {orders.Sum(o => o.Cost):C}");

			if (orders.Any())
			{
				report.AppendLine($"Средняя стоимость: {orders.Average(o => o.Cost):C}");
			}

			return report.ToString();
		}

		/// <summary>
		/// Сгенерировать отчет по статусам
		/// </summary>
		private string GenerateStatusReport(List<Order> orders, DateTime startDate, DateTime endDate)
		{
			var report = new StringBuilder();
			report.AppendLine("ОТЧЕТ: Заказы по статусу");
			report.AppendLine($"Период: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}");

			var grouped = orders.GroupBy(o => o.Status)
							   .OrderBy(g => g.Key);

			foreach (var group in grouped)
			{
				report.AppendLine($"\nСтатус: {GetStatusText(group.Key)}");
				report.AppendLine($"Количество: {group.Count()}");
				report.AppendLine($"Общая стоимость: {group.Sum(o => o.Cost):C}");
			}

			return report.ToString();
		}

		/// <summary>
		/// Сгенерировать отчет по приоритетам
		/// </summary>
		private string GeneratePriorityReport(List<Order> orders, DateTime startDate, DateTime endDate)
		{
			var report = new StringBuilder();
			report.AppendLine("ОТЧЕТ: Заказы по приоритету");
			report.AppendLine($"Период: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}");

			var priorities = new[] { "Высокий", "Средний", "Низкий" };

			foreach (var priority in priorities)
			{
				var priorityOrders = orders.Where(o => o.Priority == priority).ToList();
				if (priorityOrders.Any())
				{
					report.AppendLine($"\nПриоритет: {priority}");
					report.AppendLine($"Количество: {priorityOrders.Count}");
					report.AppendLine($"Общая стоимость: {priorityOrders.Sum(o => o.Cost):C}");
				}
			}

			return report.ToString();
		}

		/// <summary>
		/// Сгенерировать отчет по просроченным заказам
		/// </summary>
		private string GenerateOverdueReport(DateTime startDate, DateTime endDate)
		{
			var overdueOrders = _databaseFacade.GetOverdueOrders();
			var report = new StringBuilder();

			report.AppendLine("ОТЧЕТ: Просроченные заказы");
			report.AppendLine($"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm}");
			report.AppendLine($"Всего просроченных заказов: {overdueOrders.Count}");

			if (overdueOrders.Any())
			{
				report.AppendLine($"Общая сумма просрочки: {overdueOrders.Sum(o => o.Cost):C}");

				foreach (var order in overdueOrders)
				{
					var daysOverdue = (DateTime.Now - order.Deadline).Days;
					report.AppendLine($"\nЗаказ: {order.OrderNumber}");
					report.AppendLine($"Клиент: {order.ClientName}");
					report.AppendLine($"Просрочено дней: {daysOverdue}");
				}
			}

			return report.ToString();
		}

		/// <summary>
		/// Сгенерировать финансовый отчет
		/// </summary>
		private string GenerateFinancialReport(List<Order> orders, DateTime startDate, DateTime endDate)
		{
			var report = new StringBuilder();
			report.AppendLine("ФИНАНСОВЫЙ ОТЧЕТ");
			report.AppendLine($"Период: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}");
			report.AppendLine($"\nОбщая выручка: {orders.Sum(o => o.Cost):C}");
			report.AppendLine($"Количество заказов: {orders.Count}");

			if (orders.Any())
			{
				report.AppendLine($"Средний чек: {orders.Average(o => o.Cost):C}");

				// Группировка по месяцам
				var byMonth = orders.GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
								   .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month);

				report.AppendLine("\nДинамика по месяцам:");
				foreach (var group in byMonth)
				{
					var monthName = new DateTime(group.Key.Year, group.Key.Month, 1).ToString("MMMM yyyy");
					report.AppendLine($"{monthName,-20}: {group.Sum(o => o.Cost),12:C} ({group.Count()} зак.)");
				}
			}

			return report.ToString();
		}

		/// <summary>
		/// Сгенерировать отчет по активным заказам
		/// </summary>
		private string GenerateActiveOrdersReport(List<Order> orders, DateTime startDate, DateTime endDate)
		{
			var activeOrders = orders.Where(o => o.Status != OrderStatus.Completed &&
												o.Status != OrderStatus.Shipped)
									.ToList();

			var report = new StringBuilder();
			report.AppendLine("ОТЧЕТ: Активные заказы");
			report.AppendLine($"Период: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}");
			report.AppendLine($"Активных заказов: {activeOrders.Count}");
			report.AppendLine($"Общая стоимость активных заказов: {activeOrders.Sum(o => o.Cost):C}");

			return report.ToString();
		}

		/// <summary>
		/// Получить текстовое представление статуса
		/// </summary>
		private string GetStatusText(OrderStatus status)
		{
			return status switch
			{
				OrderStatus.New => "Новый",
				OrderStatus.InProgress => "В работе",
				OrderStatus.QualityControl => "На проверке",
				OrderStatus.Completed => "Завершен",
				OrderStatus.Shipped => "Отгружен",
				_ => "Неизвестно"
			};
		}
	}
}