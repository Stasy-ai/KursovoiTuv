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
	public partial class LoginWindow : Window
	{
		public LoginWindow()
		{
			InitializeComponent();

			Loaded += (s, e) => UsernameTextBox.Focus();

			KeyDown += LoginWindow_KeyDown;
		}


		private void LoginWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				LoginButton_Click(sender, e);
			}
			else if (e.Key == Key.Escape)
			{
				ExitButton_Click(sender, e);
			}
		}


		private void LoginButton_Click(object sender, RoutedEventArgs e)
		{
			string username = UsernameTextBox.Text;
			string password = PasswordBox.Password;

			System.Diagnostics.Debug.WriteLine($"Попытка входа: {username} / {password}");

			var mainWindow = new MainWindow();
			mainWindow.Show();

			this.Close();
		}


		private void ExitButton_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}
