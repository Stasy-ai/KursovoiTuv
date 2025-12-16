using KursovoiTuv.Models;
using KursovoiTuv.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace KursovoiTuv.ViewModel
{
	public class MainViewModel : INotifyPropertyChanged
	{
		private Order _selectedOrder;
		private Order _currentOrder;
		private string _newOrderClient = "Новый клиент";
		private string _newOrderProduct = "Новое изделие";
		private string _newOrderQuantity = "1";
		private string _newOrderPriority;
		private bool _isEditing;
		private int _nextOrderId;

		public MainViewModel()
		{
			Orders = new ObservableCollection<Order>();
			InitializeCommands();
			LoadSampleData();
			NewOrderPriority = Priority.Medium;

			// Инициализируем текущий заказ для формы редактирования
			CurrentOrder = new Order();
			_isEditing = false;
			_nextOrderId = Orders.Count > 0 ? GetMaxOrderId() + 1 : 1;
		}

		private const decimal BaseCostPerUnit = 1500m;
		private const int DefaultDeadlineDays = 30;

		public static class Priority
		{
			public const string High = "Высокий";
			public const string Medium = "Средний";
			public const string Low = "Низкий";
		}

		public ObservableCollection<Order> Orders { get; }

		public Order SelectedOrder
		{
			get => _selectedOrder;
			set
			{
				_selectedOrder = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(CanExecuteNextStatus));
				OnPropertyChanged(nameof(CanExecutePreviousStatus));
				OnPropertyChanged(nameof(CanEditOrder));
				OnPropertyChanged(nameof(CanDeleteOrder));
			}
		}

		public Order CurrentOrder
		{
			get => _currentOrder;
			set => SetProperty(ref _currentOrder, value);
		}

		public bool IsEditing
		{
			get => _isEditing;
			set
			{
				if (SetProperty(ref _isEditing, value))
				{
					OnPropertyChanged(nameof(EditModeTitle));
					OnPropertyChanged(nameof(CanAddOrder));
				}
			}
		}

		public string EditModeTitle => IsEditing ? "Редактирование заказа" : "Новый заказ";

		public bool CanEditOrder => SelectedOrder != null && !IsEditing;
		public bool CanDeleteOrder => SelectedOrder != null;
		public bool CanAddOrder => !IsEditing;

		public string NewOrderClient
		{
			get => _newOrderClient;
			set => SetProperty(ref _newOrderClient, value);
		}

		public string NewOrderProduct
		{
			get => _newOrderProduct;
			set => SetProperty(ref _newOrderProduct, value);
		}

		public string NewOrderQuantity
		{
			get => _newOrderQuantity;
			set => SetProperty(ref _newOrderQuantity, value);
		}

		public string NewOrderPriority
		{
			get => _newOrderPriority;
			set => SetProperty(ref _newOrderPriority, value);
		}

		public ICommand AddOrderCommand { get; private set; }
		public ICommand NextStatusCommand { get; private set; }
		public ICommand PreviousStatusCommand { get; private set; }
		public ICommand CalculateCostCommand { get; private set; }
		public ICommand OpenAboutCommand { get; private set; }
		public ICommand EditOrderCommand { get; private set; }
		public ICommand UpdateOrderCommand { get; private set; }
		public ICommand DeleteOrderCommand { get; private set; }
		public ICommand CancelEditCommand { get; private set; }

		private void InitializeCommands()
		{
			AddOrderCommand = new RelayCommand(AddOrder, _ => CanAddOrder);
			NextStatusCommand = new RelayCommand(NextOrderStatus, CanExecuteNextStatus);
			PreviousStatusCommand = new RelayCommand(PreviousOrderStatus, CanExecutePreviousStatus);
			CalculateCostCommand = new RelayCommand(CalculateCost);
			OpenAboutCommand = new RelayCommand(OpenAboutWindow);
			EditOrderCommand = new RelayCommand(EditOrder, _ => CanEditOrder);
			UpdateOrderCommand = new RelayCommand(UpdateOrder, _ => IsEditing);
			DeleteOrderCommand = new RelayCommand(DeleteOrder, _ => CanDeleteOrder);
			CancelEditCommand = new RelayCommand(CancelEdit, _ => IsEditing);
		}

		private void LoadSampleData()
		{
			var sampleOrders = new[]
			{
				new Order
				{
					Id = 1,
					OrderNumber = "MP-2024-001",
					ClientName = "ООО 'СтройМеталл'",
					ProductDescription = "Металлоконструкции L-12",
					Quantity = 50,
					OrderDate = DateTime.Now.AddDays(-5),
					Deadline = DateTime.Now.AddDays(15),
					Cost = 125000m,
					Priority = Priority.High,
					Status = OrderStatus.New
				},
				new Order
				{
					Id = 2,
					OrderNumber = "MP-2024-002",
					ClientName = "ЗАО 'МашПром'",
					ProductDescription = "Валы стальные Ø80mm",
					Quantity = 100,
					OrderDate = DateTime.Now.AddDays(-2),
					Deadline = DateTime.Now.AddDays(25),
					Cost = 89000m,
					Priority = Priority.Medium,
					Status = OrderStatus.InProgress
				}
			};

			foreach (var order in sampleOrders)
			{
				Orders.Add(order);
			}

			SelectedOrder = Orders.Count > 0 ? Orders[0] : null;
		}

		private int GetMaxOrderId()
		{
			int maxId = 0;
			foreach (var order in Orders)
			{
				if (order.Id > maxId)
					maxId = order.Id;
			}
			return maxId;
		}

		private void AddOrder(object parameter)
		{
			if (!ValidateNewOrder()) return;

			var newOrder = CreateNewOrder();
			CalculateCostForOrder(newOrder);

			Orders.Add(newOrder);
			SelectedOrder = newOrder;
			_nextOrderId++;
			ResetOrderForm();
		}

		private bool ValidateNewOrder()
		{
			if (!int.TryParse(NewOrderQuantity, out int quantity) || quantity <= 0)
			{
				MessageBox.Show("Введите корректное количество", "Ошибка",
					MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
			return true;
		}

		private Order CreateNewOrder()
		{
			return new Order
			{
				Id = _nextOrderId,
				OrderNumber = GenerateOrderNumber(),
				ClientName = NewOrderClient,
				ProductDescription = NewOrderProduct,
				Quantity = int.Parse(NewOrderQuantity),
				OrderDate = DateTime.Now,
				Deadline = DateTime.Now.AddDays(DefaultDeadlineDays),
				Priority = NewOrderPriority,
				Status = OrderStatus.New,
				Cost = 0
			};
		}

		private string GenerateOrderNumber() => $"MP-{DateTime.Now:yyyyMMdd}-{_nextOrderId:000}";

		private void ResetOrderForm()
		{
			NewOrderClient = "Новый клиент";
			NewOrderProduct = "Новое изделие";
			NewOrderQuantity = "1";
			NewOrderPriority = Priority.Medium;
		}

		private void CalculateCostForOrder(Order order)
		{
			if (order == null) return;

			decimal multiplier = order.Priority switch
			{
				Priority.High => 1.2m,
				Priority.Low => 0.8m,
				_ => 1.0m
			};

			order.Cost = order.Quantity * BaseCostPerUnit * multiplier;
		}

		private void CalculateCost(object parameter) => CalculateCostForOrder(SelectedOrder);

		private bool CanExecuteNextStatus(object parameter) => SelectedOrder?.CanMoveNext ?? false;

		private void NextOrderStatus(object parameter)
		{
			if (SelectedOrder?.CanMoveNext == true)
			{
				SelectedOrder.NextStatus();
				OnPropertyChanged(nameof(CanExecuteNextStatus));
				OnPropertyChanged(nameof(CanExecutePreviousStatus));
			}
		}

		private bool CanExecutePreviousStatus(object parameter) => SelectedOrder?.CanMovePrevious ?? false;

		private void PreviousOrderStatus(object parameter)
		{
			if (SelectedOrder?.CanMovePrevious == true)
			{
				SelectedOrder.PreviousStatus();
				OnPropertyChanged(nameof(CanExecuteNextStatus));
				OnPropertyChanged(nameof(CanExecutePreviousStatus));
			}
		}

		private void EditOrder(object parameter)
		{
			if (SelectedOrder == null) return;

			// Создаем копию заказа для редактирования
			CurrentOrder = new Order
			{
				Id = SelectedOrder.Id,
				OrderNumber = SelectedOrder.OrderNumber,
				ClientName = SelectedOrder.ClientName,
				ProductDescription = SelectedOrder.ProductDescription,
				Quantity = SelectedOrder.Quantity,
				OrderDate = SelectedOrder.OrderDate,
				Deadline = SelectedOrder.Deadline,
				Cost = SelectedOrder.Cost,
				Priority = SelectedOrder.Priority,
				Status = SelectedOrder.Status
			};

			IsEditing = true;
		}

		private void UpdateOrder(object parameter)
		{
			if (SelectedOrder == null || CurrentOrder == null) return;

			if (!ValidateEditOrder()) return;

			// Обновляем выбранный заказ данными из формы редактирования
			SelectedOrder.ClientName = CurrentOrder.ClientName;
			SelectedOrder.ProductDescription = CurrentOrder.ProductDescription;
			SelectedOrder.Quantity = CurrentOrder.Quantity;
			SelectedOrder.Deadline = CurrentOrder.Deadline;
			SelectedOrder.Priority = CurrentOrder.Priority;
			SelectedOrder.Status = CurrentOrder.Status;

			// Пересчитываем стоимость при изменении приоритета или количества
			if (SelectedOrder.Priority != CurrentOrder.Priority || SelectedOrder.Quantity != CurrentOrder.Quantity)
			{
				CalculateCostForOrder(SelectedOrder);
			}

			// Обновляем привязки
			OnPropertyChanged(nameof(SelectedOrder));
			CancelEdit(parameter);
		}

		private bool ValidateEditOrder()
		{
			if (CurrentOrder.Quantity <= 0)
			{
				MessageBox.Show("Количество должно быть больше 0", "Ошибка",
					MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			if (CurrentOrder.Deadline < CurrentOrder.OrderDate)
			{
				MessageBox.Show("Срок выполнения не может быть раньше даты заказа", "Ошибка",
					MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			return true;
		}

		private void DeleteOrder(object parameter)
		{
			if (SelectedOrder == null) return;

			var result = MessageBox.Show($"Вы уверены, что хотите удалить заказ {SelectedOrder.OrderNumber}?",
				"Подтверждение удаления",
				MessageBoxButton.YesNo,
				MessageBoxImage.Question);

			if (result == MessageBoxResult.Yes)
			{
				int index = Orders.IndexOf(SelectedOrder);
				Orders.Remove(SelectedOrder);

				// Выбираем следующий заказ, если есть
				if (Orders.Count > 0)
				{
					if (index >= Orders.Count)
						index = Orders.Count - 1;
					SelectedOrder = Orders[index];
				}
				else
				{
					SelectedOrder = null;
				}

				OnPropertyChanged(nameof(Orders));
			}
		}

		private void CancelEdit(object parameter)
		{
			IsEditing = false;
			CurrentOrder = new Order(); // Сбрасываем форму редактирования
		}

		private void OpenAboutWindow(object parameter)
		{
			var aboutWindow = new AboutWindow { Owner = Application.Current.MainWindow };
			aboutWindow.ShowDialog();
		}

		private void ExitApplication(object parameter) => Application.Current.Shutdown();

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (Equals(field, value)) return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}
	}
}