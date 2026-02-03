using System.Windows;

namespace KursovoiTuv.Views
{
	public partial class LoginWindow : Window
	{
		public LoginWindow()
		{
			InitializeComponent();
			Loaded += (s, e) => UsernameBox.Focus();
		}

		private void LoginButton_Click(object sender, RoutedEventArgs e)
		{
			string username = UsernameBox.Text.Trim();
			string password = PasswordBox.Password;

			// Простейшая проверка - ДЕМО РЕЖИМ
			if ((username == "admin" || username == "manager" || username == "user")
				&& password == "123")
			{
				// Успешный вход
				var mainWindow = new MainWindow();
				mainWindow.Show();
				this.Close();
			}
			else
			{
				// Ошибка
				ErrorText.Text = "Неверный логин или пароль";
				ErrorText.Visibility = Visibility.Visible;
				PasswordBox.Focus();
				PasswordBox.SelectAll();
			}
		}

		private void ExitButton_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}