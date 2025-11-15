using KursovoiTuv.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KursovoiTuv.ViewModel
{
	internal class MainViewModel : INotifyPropertyChanged
	{
		private ObservableCollection<Order> _orders;
		private Order _selectedOrder;

		public MainViewModel()
		{
			Orders = new ObservableCollection<Order>();
			LoadSampleData();

			//AddOrderCommand = new RelayCommand(AddOrder);
			//UpdateStatusCommand = new RelayCommand(UpdateOrderStatus);
			//CalculateCostCommand = new RelayCommand(CalculateCost);
		}

		public ObservableCollection<Order> Orders
		{
			get => _orders;
			set { _orders = value; OnPropertyChanged(); }
		}

		public Order SelectedOrder
		{
			get => _selectedOrder;
			set { _selectedOrder = value; OnPropertyChanged(); }
		}

		public ICommand AddOrderCommand { get; }
		public ICommand UpdateStatusCommand { get; }
		public ICommand CalculateCostCommand { get; }

		private void LoadSampleData()
		{
			Orders.Add(new Order
			{
				Id = 1,
				OrderNumber = "MP-2024-001",
				ClientName = "ООО 'СтройМеталл'",
				ProductDescription = "Металлоконструкции L-12",
				Quantity = 50,
				OrderDate = DateTime.Now.AddDays(-5),
				Deadline = DateTime.Now.AddDays(15),
				Cost = 125000m,
				Status = OrderStatus.InProgress,
				Priority = "Высокий"
			});

			Orders.Add(new Order
			{
				Id = 2,
				OrderNumber = "MP-2024-002",
				ClientName = "ЗАО 'МашПром'",
				ProductDescription = "Валы стальные Ø80mm",
				Quantity = 100,
				OrderDate = DateTime.Now.AddDays(-2),
				Deadline = DateTime.Now.AddDays(25),
				Cost = 89000m,
				Status = OrderStatus.New,
				Priority = "Средний"
			});
		}

		private void AddOrder(object obj)
		{
			var newOrder = new Order
			{
				Id = Orders.Count + 1,
				OrderNumber = $"MP-2024-{Orders.Count + 1:000}",
				ClientName = "Новый клиент",
				ProductDescription = "Новое изделие",
				Quantity = 1,
				OrderDate = DateTime.Now,
				Deadline = DateTime.Now.AddDays(30),
				Status = OrderStatus.New,
				Priority = "Низкий"
			};

			Orders.Add(newOrder);
			SelectedOrder = newOrder;
		}

		private void UpdateOrderStatus(object obj)
		{
			if (SelectedOrder != null && SelectedOrder.Status < OrderStatus.Shipped)
			{
				SelectedOrder.Status++;
			}
		}

		private void CalculateCost(object obj)
		{
			if (SelectedOrder != null)
			{
				// Упрощенный расчет себестоимости
				SelectedOrder.Cost = SelectedOrder.Quantity * 1500 * (SelectedOrder.Priority == "Высокий" ? 1.2m : 1m);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

