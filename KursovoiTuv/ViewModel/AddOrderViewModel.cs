using KursovoiTuv.Models;
using KursovoiTuv.Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace KursovoiTuv.ViewModel
{
	/// <summary>
	/// ViewModel для добавления/редактирования заказа
	/// </summary>
	/// <remarks>
	/// Управляет данными формы заказа и валидацией ввода
	/// </remarks>
	public class AddOrderViewModel : ObservableObject
	{
		private readonly IOrderService _orderService;
		private readonly Order _originalOrder;
		private bool _isEditMode;

		private string _clientName;
		private string _productDescription;
		private int _quantity = 1;
		private string _priority = "Средний";
		private DateTime _deadline;
		private decimal _cost;

		/// <summary>
		/// Конструктор для добавления нового заказа
		/// </summary>
		/// <param name="orderService">Сервис работы с заказами</param>
		public AddOrderViewModel(IOrderService orderService)
		{
			_orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
			_deadline = DateTime.Now.AddDays(30);
			InitializeCommands();
		}

		/// <summary>
		/// Конструктор для редактирования существующего заказа
		/// </summary>
		/// <param name="orderService">Сервис работы с заказами</param>
		/// <param name="order">Заказ для редактирования</param>
		public AddOrderViewModel(IOrderService orderService, Order order)
			: this(orderService)
		{
			_originalOrder = order ?? throw new ArgumentNullException(nameof(order));
			_isEditMode = true;

			// Заполняем поля данными из заказа
			ClientName = order.ClientName;
			ProductDescription = order.ProductDescription;
			Quantity = order.Quantity;
			Priority = order.Priority;
			Deadline = order.Deadline;
			Cost = order.Cost;
		}

		/// <summary>
		/// Имя клиента
		/// </summary>
		public string ClientName
		{
			get => _clientName;
			set
			{
				if (SetProperty(ref _clientName, value))
				{
					OnPropertyChanged(nameof(CanSave));
					CalculateEstimatedCost();
				}
			}
		}

		/// <summary>
		/// Описание изделия
		/// </summary>
		public string ProductDescription
		{
			get => _productDescription;
			set
			{
				if (SetProperty(ref _productDescription, value))
				{
					OnPropertyChanged(nameof(CanSave));
				}
			}
		}

		/// <summary>
		/// Количество изделий
		/// </summary>
		public int Quantity
		{
			get => _quantity;
			set
			{
				if (SetProperty(ref _quantity, value))
				{
					OnPropertyChanged(nameof(CanSave));
					CalculateEstimatedCost();
				}
			}
		}

		/// <summary>
		/// Приоритет заказа
		/// </summary>
		public string Priority
		{
			get => _priority;
			set
			{
				if (SetProperty(ref _priority, value))
				{
					CalculateEstimatedCost();
				}
			}
		}

		/// <summary>
		/// Срок выполнения
		/// </summary>
		public DateTime Deadline
		{
			get => _deadline;
			set
			{
				if (SetProperty(ref _deadline, value))
				{
					OnPropertyChanged(nameof(CanSave));
				}
			}
		}

		/// <summary>
		/// Стоимость заказа
		/// </summary>
		public decimal Cost
		{
			get => _cost;
			set => SetProperty(ref _cost, value);
		}

		/// <summary>
		/// Предварительно рассчитанная стоимость
		/// </summary>
		public decimal EstimatedCost { get; private set; }

		/// <summary>
		/// Можно ли сохранить заказ
		/// </summary>
		public bool CanSave =>
			!string.IsNullOrWhiteSpace(ClientName) &&
			!string.IsNullOrWhiteSpace(ProductDescription) &&
			Quantity > 0 &&
			Deadline >= DateTime.Today;

		// Команды
		public ICommand SaveCommand { get; private set; }
		public ICommand CancelCommand { get; private set; }

		/// <summary>
		/// Событие сохранения заказа
		/// </summary>
		public event Action OrderSaved;

		/// <summary>
		/// Инициализация команд
		/// </summary>
		private void InitializeCommands()
		{
			SaveCommand = new RelayCommand(SaveOrder, _ => CanSave);
			CancelCommand = new RelayCommand(Cancel);
		}

		/// <summary>
		/// Рассчитать предварительную стоимость
		/// </summary>
		private void CalculateEstimatedCost()
		{
			if (Quantity > 0)
			{
				// Используем OrderService для расчета
				var orderService = new OrderService(); // или передай через DI
				EstimatedCost = orderService.CalculateOrderCost(Quantity, Priority);

				OnPropertyChanged(nameof(EstimatedCost));

				if (!_isEditMode)
				{
					Cost = EstimatedCost;
				}
			}
		}

		/// <summary>
		/// Сохранить заказ
		/// </summary>
		private void SaveOrder(object parameter)
		{
			try
			{
				var order = CreateOrderFromInput();

				if (_isEditMode)
				{
					_orderService.UpdateOrder(order);
					MessageBox.Show("Заказ обновлен", "Успех",
						MessageBoxButton.OK, MessageBoxImage.Information);
				}
				else
				{
					_orderService.CreateOrder(order);
					MessageBox.Show("Заказ создан", "Успех",
						MessageBoxButton.OK, MessageBoxImage.Information);
				}

				OrderSaved?.Invoke();
				CloseWindow();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка сохранения: {ex.Message}",
					"Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/// <summary>
		/// Создать объект заказа из введенных данных
		/// </summary>
		private Order CreateOrderFromInput()
		{
			var order = new Order
			{
				ClientName = ClientName.Trim(),
				ProductDescription = ProductDescription.Trim(),
				Quantity = Quantity,
				Priority = Priority,
				Deadline = Deadline,
				Cost = Cost,
				OrderDate = _isEditMode ? _originalOrder.OrderDate : DateTime.Now,
				Status = _isEditMode ? _originalOrder.Status : OrderStatus.New
			};

			if (_isEditMode)
			{
				order.Id = _originalOrder.Id;
				order.OrderNumber = _originalOrder.OrderNumber;
			}
			else
			{
				// Номер сгенерируется автоматически в фасаде
			}

			return order;
		}

		/// <summary>
		/// Отменить операцию
		/// </summary>
		private void Cancel(object parameter)
		{
			CloseWindow();
		}

		/// <summary>
		/// Закрыть окно
		/// </summary>
		private void CloseWindow()
		{
			// Поиск окна по DataContext
			foreach (Window window in Application.Current.Windows)
			{
				if (window.DataContext == this)
				{
					window.DialogResult = true;
					window.Close();
					break;
				}
			}
		}
	}
}