using System;
using System.Windows;
using System.Windows.Input;

namespace KursovoiTuv.ViewModel
{
	public class LoginViewModel : ObservableObject
	{
		private string _username = "admin";
		private string _password = "123";
		private string _errorMessage;

		public LoginViewModel()
		{
			LoginCommand = new RelayCommand(Login, (parameter) => CanLogin);
			ExitCommand = new RelayCommand(Exit);
		}

		public string Username
		{
			get => _username;
			set => SetProperty(ref _username, value);
		}

		public string Password
		{
			get => _password;
			set => SetProperty(ref _password, value);
		}

		public string ErrorMessage
		{
			get => _errorMessage;
			set => SetProperty(ref _errorMessage, value);
		}

		public bool CanLogin => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);

		public ICommand LoginCommand { get; }
		public ICommand ExitCommand { get; }

		public event Action<bool> LoginCompleted;

		private void Login(object parameter)
		{
			ErrorMessage = null;

			// Простая проверка (демо-режим)
			if (Username == "admin" && Password == "123")
			{
				// Успешная авторизация
				App.CurrentUser = new Models.User
				{
					Username = Username,
					Role = "Admin"
				};
				LoginCompleted?.Invoke(true);
			}
			else if (Username == "manager" && Password == "123")
			{
				App.CurrentUser = new Models.User
				{
					Username = Username,
					Role = "Manager"
				};
				LoginCompleted?.Invoke(true);
			}
			else if (Username == "user" && Password == "123")
			{
				App.CurrentUser = new Models.User
				{
					Username = Username,
					Role = "User"
				};
				LoginCompleted?.Invoke(true);
			}
			else
			{
				ErrorMessage = "Неверный логин или пароль";
				LoginCompleted?.Invoke(false);
			}
		}

		private void Exit(object parameter)
		{
			Application.Current.Shutdown();
		}
	}
}