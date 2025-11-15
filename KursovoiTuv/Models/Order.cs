using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KursovoiTuv.Models
{
	internal class Order : INotifyPropertyChanged
	{
		private OrderStatus _status;

		public int Id { get; set; }
		public string OrderNumber { get; set; }
		public string ClientName { get; set; }
		public string ProductDescription { get; set; }
		public int Quantity { get; set; }
		public DateTime OrderDate { get; set; }
		public DateTime Deadline { get; set; }
		public decimal Cost { get; set; }
		public OrderStatus Status
		{
			get => _status;
			set { _status = value; OnPropertyChanged(); }
		}
		public string Priority { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	public enum OrderStatus
	{
		New,
		InProgress,
		QualityControl,
		Completed,
		Shipped
	}
}
