using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KursovoiTuv.Models;

namespace KursovoiTuv.Patterns.State
{
	/// <summary>
	/// Интерфейс состояния заказа (паттерн State)
	/// </summary>
	/// <remarks>
	/// Определяет поведение заказа в зависимости от его текущего состояния.
	/// Каждое состояние знает, в какое состояние можно перейти дальше.
	/// </remarks>
	public interface IOrderState
	{
		/// <summary>
		/// Название состояния
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Можно ли перейти к следующему состоянию
		/// </summary>
		bool CanMoveToNext { get; }

		/// <summary>
		/// Можно ли вернуться к предыдущему состоянию
		/// </summary>
		bool CanMoveToPrevious { get; }

		/// <summary>
		/// Получить следующее состояние
		/// </summary>
		/// <returns>Следующее состояние заказа</returns>
		IOrderState NextState();

		/// <summary>
		/// Получить предыдущее состояние
		/// </summary>
		/// <returns>Предыдущее состояние заказа</returns>
		IOrderState PreviousState();

		/// <summary>
		/// Применить состояние к заказу
		/// </summary>
		/// <param name="order">Заказ для применения состояния</param>
		void ApplyStatus(Order order);
	}

	/// <summary>
	/// Состояние: Новый заказ
	/// </summary>
	public class NewOrderState : IOrderState
	{
		public string Name => "Новый";
		public bool CanMoveToNext => true;
		public bool CanMoveToPrevious => false;

		public IOrderState NextState() => new InProgressOrderState();
		public IOrderState PreviousState() => this;

		public void ApplyStatus(Order order)
		{
			order.Status = OrderStatus.New;
		}
	}

	/// <summary>
	/// Состояние: Заказ в работе
	/// </summary>
	public class InProgressOrderState : IOrderState
	{
		public string Name => "В работе";
		public bool CanMoveToNext => true;
		public bool CanMoveToPrevious => true;

		public IOrderState NextState() => new QualityControlOrderState();
		public IOrderState PreviousState() => new NewOrderState();

		public void ApplyStatus(Order order)
		{
			order.Status = OrderStatus.InProgress;
		}
	}

	/// <summary>
	/// Состояние: Заказ на проверке качества
	/// </summary>
	public class QualityControlOrderState : IOrderState
	{
		public string Name => "На проверке";
		public bool CanMoveToNext => true;
		public bool CanMoveToPrevious => true;

		public IOrderState NextState() => new CompletedOrderState();
		public IOrderState PreviousState() => new InProgressOrderState();

		public void ApplyStatus(Order order)
		{
			order.Status = OrderStatus.QualityControl;
		}
	}

	/// <summary>
	/// Состояние: Заказ завершен
	/// </summary>
	public class CompletedOrderState : IOrderState
	{
		public string Name => "Завершен";
		public bool CanMoveToNext => true;
		public bool CanMoveToPrevious => true;

		public IOrderState NextState() => new ShippedOrderState();
		public IOrderState PreviousState() => new QualityControlOrderState();

		public void ApplyStatus(Order order)
		{
			order.Status = OrderStatus.Completed;
		}
	}

	/// <summary>
	/// Состояние: Заказ отгружен
	/// </summary>
	public class ShippedOrderState : IOrderState
	{
		public string Name => "Отгружен";
		public bool CanMoveToNext => false;
		public bool CanMoveToPrevious => true;

		public IOrderState NextState() => this;
		public IOrderState PreviousState() => new CompletedOrderState();

		public void ApplyStatus(Order order)
		{
			order.Status = OrderStatus.Shipped;
		}
	}

	/// <summary>
	/// Фабрика состояний заказа
	/// </summary>
	/// <remarks>
	/// Создает соответствующий объект состояния на основе статуса заказа
	/// </remarks>
	public static class OrderStateFactory
	{
		/// <summary>
		/// Создает состояние на основе статуса заказа
		/// </summary>
		/// <param name="status">Статус заказа</param>
		/// <returns>Объект состояния</returns>
		public static IOrderState CreateState(OrderStatus status)
		{
			return status switch
			{
				OrderStatus.New => new NewOrderState(),
				OrderStatus.InProgress => new InProgressOrderState(),
				OrderStatus.QualityControl => new QualityControlOrderState(),
				OrderStatus.Completed => new CompletedOrderState(),
				OrderStatus.Shipped => new ShippedOrderState(),
				_ => new NewOrderState()
			};
		}
	}
}
