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
		private string _newOrderClient = "Новый клиент";
		private string _newOrderProduct = "Новое изделие";
		private string _newOrderQuantity = "1";
		private string _newOrderPriority;

		public MainViewModel()
		{
			Orders = new ObservableCollection<Order>();
			InitializeCommands();
			LoadSampleData();
			NewOrderPriority = Priority.Medium;
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
			}
		}

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

		private void InitializeCommands()
		{
			AddOrderCommand = new RelayCommand(AddOrder);
			NextStatusCommand = new RelayCommand(NextOrderStatus, CanExecuteNextStatus);
			PreviousStatusCommand = new RelayCommand(PreviousOrderStatus, CanExecutePreviousStatus);
			CalculateCostCommand = new RelayCommand(CalculateCost);
			OpenAboutCommand = new RelayCommand(OpenAboutWindow);
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
					Status = OrderStatus.InProgress
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
					Status = OrderStatus.New
				}
			};

			foreach (var order in sampleOrders)
			{
				Orders.Add(order);
			}

			SelectedOrder = Orders.Count > 0 ? Orders[0] : null;
		}

		private void AddOrder(object parameter)
		{
			if (!ValidateNewOrder()) return;

			var newOrder = CreateNewOrder();
			CalculateCostForOrder(newOrder);

			Orders.Add(newOrder);
			SelectedOrder = newOrder;
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
				Id = Orders.Count + 1,
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

		private string GenerateOrderNumber() => $"MP-{DateTime.Now:yyyyMMdd}-{Orders.Count + 1:000}";

		private void ResetOrderForm()
		{
			NewOrderClient = "Новый клиент";
			NewOrderProduct = "Новое изделие";
			NewOrderQuantity = "1";
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