using System;
using System.Windows.Input;

namespace KursovoiTuv.ViewModel
{
	/// <summary>
	/// Команда для привязки в WPF (паттерн Command)
	/// </summary>
	/// <remarks>
	/// Позволяет связывать UI элементы с методами ViewModel
	/// и управлять доступностью команд
	/// </remarks>
	public class RelayCommand : ICommand
	{
		private readonly Action<object> _execute;
		private readonly Func<object, bool> _canExecute;

		/// <summary>
		/// Конструктор команды
		/// </summary>
		/// <param name="execute">Метод для выполнения</param>
		/// <param name="canExecute">Функция проверки доступности</param>
		public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
			_canExecute = canExecute;
		}

		/// <summary>
		/// Проверяет возможность выполнения команды
		/// </summary>
		public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

		/// <summary>
		/// Выполняет команду
		/// </summary>
		public void Execute(object parameter) => _execute(parameter);

		/// <summary>
		/// Событие изменения состояния доступности команды
		/// </summary>
		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		/// <summary>
		/// Вызывает обновление состояния доступности команды
		/// </summary>
		public void RaiseCanExecuteChanged()
		{
			CommandManager.InvalidateRequerySuggested();
		}
	}
}