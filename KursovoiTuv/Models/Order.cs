using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KursovoiTuv.Models
{
	/// <summary>
	/// Статусы производственного заказа
	/// </summary>
	public enum OrderStatus
	{
		/// <summary>
		/// Новый заказ
		/// </summary>
		New = 0,

		/// <summary>
		/// Заказ в работе
		/// </summary>
		InProgress = 1,

		/// <summary>
		/// Заказ на проверке качества
		/// </summary>
		QualityControl = 2,

		/// <summary>
		/// Заказ завершен
		/// </summary>
		Completed = 3,

		/// <summary>
		/// Заказ отгружен
		/// </summary>
		Shipped = 4
	}

	/// <summary>
	/// Представляет производственный заказ
	/// </summary>
	/// <remarks>
	/// Содержит информацию о заказе: клиент, изделие,
	/// количество, сроки, стоимость и статус выполнения
	/// </remarks>
	public class Order
	{
		/// <summary>
		/// Уникальный идентификатор заказа
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Номер заказа в формате MP-YYYYMMDD-XXX
		/// </summary>
		public string OrderNumber { get; set; }

		/// <summary>
		/// Наименование клиента
		/// </summary>
		public string ClientName { get; set; }

		/// <summary>
		/// Описание изделия
		/// </summary>
		public string ProductDescription { get; set; }

		/// <summary>
		/// Количество изделий
		/// </summary>
		public int Quantity { get; set; }

		/// <summary>
		/// Приоритет заказа (Высокий/Средний/Низкий)
		/// </summary>
		public string Priority { get; set; }

		/// <summary>
		/// Дата создания заказа
		/// </summary>
		public DateTime OrderDate { get; set; }

		/// <summary>
		/// Срок выполнения заказа
		/// </summary>
		public DateTime Deadline { get; set; }

		/// <summary>
		/// Стоимость заказа
		/// </summary>
		public decimal Cost { get; set; }

		/// <summary>
		/// Текущий статус заказа
		/// </summary>
		public OrderStatus Status { get; set; }

		/// <summary>
		/// Получает текстовое представление статуса
		/// </summary>
		/// <returns>Название статуса на русском языке</returns>
		public string GetStatusText()
		{
			return Status switch
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