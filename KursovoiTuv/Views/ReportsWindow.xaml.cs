using KursovoiTuv.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KursovoiTuv.Views
{
	public partial class ReportsWindow : Window
	{
		public ReportsWindow()
		{
			InitializeComponent();
			DataContext = new ReportsViewModel();

			// Подписываемся на событие закрытия
			var viewModel = DataContext as ReportsViewModel;
			if (viewModel != null)
			{
				viewModel.CloseRequested += () => this.DialogResult = true;
			}
		}
	}
}