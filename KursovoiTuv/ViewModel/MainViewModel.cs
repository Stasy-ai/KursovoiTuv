using KursovoiTuv.Data;
using KursovoiTuv.Models;
using KursovoiTuv.Patterns.State;
using KursovoiTuv.Services;
using KursovoiTuv.Views;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace KursovoiTuv.ViewModel
{
	/// <summary>
	/// Основная ViewModel приложения
	/// </summary>
	/// <remarks>
	/// Управляет отображением заказов, обработкой команд
	/// и навигацией между окнами приложения
	/// </remarks>
	public class MainViewModel : ObservableObject
	{
		private Order _selectedOrder;
		private readonly IOrderService _orderService;
		private readonly IReportService _reportService;

		/// <summary>
		/// Конструктор основной ViewModel
		/// </summary>
		/// <param name="orderService">Сервис работы с заказами</param>
		/// <param name="reportService">Сервис генерации отчетов</param>
		public MainViewModel()
		{
			// Создаем зависимости ВРУЧНУЮ
			var dbContext = new ApplicationDbContext();
			var facade = new OrderDatabaseFacade(dbContext);
			_orderService = new OrderService(facade);

			Orders = new ObservableCollection<Order>();
			InitializeCommands();
			LoadOrders();
		}

		/// <summary>
		/// Коллекция заказов для отображения
		/// </summary>
		public ObservableCollection<Order> Orders { get; }

		/// <summary>
		/// Выбранный заказ
		/// </summary>
		public Order SelectedOrder
		{
			get => _selectedOrder;
			set
			{
				if (SetProperty(ref _selectedOrder, value))
				{
					OnPropertyChanged(nameof(CanMoveToNextStatus));
					OnPropertyChanged(nameof(CanMoveToPreviousStatus));
				}
			}
		}

		/// <summary>
		/// Можно ли перейти к следующему статусу
		/// </summary>
		public bool CanMoveToNextStatus
		{
			get
			{
				if (SelectedOrder == null) return false;
				var state = OrderStateFactory.CreateState(SelectedOrder.Status);
				return state.CanMoveToNext;
			}
		}

		/// <summary>
		/// Можно ли вернуться к предыдущему статусу
		/// </summary>
		public bool CanMoveToPreviousStatus
		{
			get
			{
				if (SelectedOrder == null) return false;
				var state = OrderStateFactory.CreateState(SelectedOrder.Status);
				return state.CanMoveToPrevious;
			}
		}

		// Команды
		public ICommand AddOrderCommand { get; private set; }
		public ICommand EditOrderCommand { get; private set; }
		public ICommand DeleteOrderCommand { get; private set; }
		public ICommand NextStatusCommand { get; private set; }
		public ICommand PreviousStatusCommand { get; private set; }
		public ICommand OpenReportsCommand { get; private set; }
		public ICommand OpenAboutCommand { get; private set; }

		/// <summary>
		/// Инициализация команд
		/// </summary>
		private void InitializeCommands()
		{
			AddOrderCommand = new RelayCommand(OpenAddOrderWindow);
			EditOrderCommand = new RelayCommand(OpenEditOrderWindow, _ => SelectedOrder != null);
			DeleteOrderCommand = new RelayCommand(DeleteOrder, _ => SelectedOrder != null);
			NextStatusCommand = new RelayCommand(MoveToNextStatus, _ => CanMoveToNextStatus);
			PreviousStatusCommand = new RelayCommand(MoveToPreviousStatus, _ => CanMoveToPreviousStatus);
			OpenReportsCommand = new RelayCommand(OpenReportsWindow);
			OpenAboutCommand = new RelayCommand(OpenAboutWindow);
		}

		/// <summary>
		/// Загрузить заказы из базы данных
		/// </summary>
		private void LoadOrders()
		{
			try
			{
				var orders = _orderService.GetAllOrders();
				Orders.Clear();

				foreach (var order in orders)
				{
					Orders.Add(order);
				}

				SelectedOrder = Orders.Count > 0 ? Orders[0] : null;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}",
					"Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/// <summary>
		/// Открыть окно добавления заказа
		/// </summary>
		private void OpenAddOrderWindow(object parameter)
		{
			try
			{
				// Простое открытие окна
				var window = new AddOrderWindow();

				// Создаем ViewModel
				var viewModel = new AddOrderViewModel(_orderService);
				window.DataContext = viewModel;

				window.ShowDialog();

				// Обновляем список после закрытия
				LoadOrders();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка открытия окна: {ex.Message}", "Ошибка");
			}
		}

		/// <summary>
		/// Открыть окно редактирования заказа
		/// </summary>
		private void OpenEditOrderWindow(object parameter)
		{
			if (SelectedOrder == null) return;

			var editOrderViewModel = new AddOrderViewModel(_orderService, SelectedOrder);
			editOrderViewModel.OrderSaved += OnOrderSaved;

			var window = new AddOrderWindow { DataContext = editOrderViewModel };
			window.ShowDialog();
		}

		/// <summary>
		/// Удалить выбранный заказ
		/// </summary>
		private void DeleteOrder(object parameter)
		{
			if (SelectedOrder == null) return;

			var result = MessageBox.Show($"Удалить заказ {SelectedOrder.OrderNumber}?",
				"Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (result == MessageBoxResult.Yes)
			{
				try
				{
					bool success = _orderService.DeleteOrder(SelectedOrder.Id);

					if (success)
					{
						Orders.Remove(SelectedOrder);
						MessageBox.Show("Заказ удален", "Успех",
							MessageBoxButton.OK, MessageBoxImage.Information);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Ошибка удаления: {ex.Message}",
						"Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		/// <summary>
		/// Перейти к следующему статусу
		/// </summary>
		private void MoveToNextStatus(object parameter)
		{
			if (SelectedOrder == null) return;

			var currentState = OrderStateFactory.CreateState(SelectedOrder.Status);
			if (currentState.CanMoveToNext)
			{
				var nextState = currentState.NextState();
				nextState.ApplyStatus(SelectedOrder);

				try
				{
					_orderService.UpdateOrder(SelectedOrder);
					OnPropertyChanged(nameof(CanMoveToNextStatus));
					OnPropertyChanged(nameof(CanMoveToPreviousStatus));
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Ошибка обновления статуса: {ex.Message}",
						"Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		/// <summary>
		/// Вернуться к предыдущему статусу
		/// </summary>
		private void MoveToPreviousStatus(object parameter)
		{
			if (SelectedOrder == null) return;

			var currentState = OrderStateFactory.CreateState(SelectedOrder.Status);
			if (currentState.CanMoveToPrevious)
			{
				var previousState = currentState.PreviousState();
				previousState.ApplyStatus(SelectedOrder);

				try
				{
					_orderService.UpdateOrder(SelectedOrder);
					OnPropertyChanged(nameof(CanMoveToNextStatus));
					OnPropertyChanged(nameof(CanMoveToPreviousStatus));
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Ошибка обновления статуса: {ex.Message}",
						"Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		/// <summary>
		/// Открыть окно отчетов
		/// </summary>
		private void OpenReportsWindow(object parameter)
		{
			var window = new ReportsWindow();
			window.ShowDialog();
		}

		/// <summary>
		/// Открыть окно "О программе"
		/// </summary>
		private void OpenAboutWindow(object parameter)
		{
			var window = new AboutWindow();
			window.ShowDialog();
		}

		/// <summary>
		/// Обработчик сохранения заказа
		/// </summary>
		private void OnOrderSaved()
		{
			LoadOrders(); // Перезагружаем список заказов
		}
	}
}