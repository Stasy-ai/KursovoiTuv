using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KursovoiTuv.ViewModel
{
	/// <summary>
	/// Базовый класс для объектов, поддерживающих уведомления об изменении свойств
	/// </summary>
	/// <remarks>
	/// Реализует интерфейс INotifyPropertyChanged для поддержки привязки данных в WPF
	/// </remarks>
	public class ObservableObject : INotifyPropertyChanged
	{
		/// <summary>
		/// Событие изменения свойства
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Вызывает событие изменения свойства
		/// </summary>
		/// <param name="propertyName">Имя изменившегося свойства</param>
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Устанавливает значение свойства и вызывает событие изменения
		/// </summary>
		/// <typeparam name="T">Тип свойства</typeparam>
		/// <param name="field">Поле свойства</param>
		/// <param name="value">Новое значение</param>
		/// <param name="propertyName">Имя свойства</param>
		/// <returns>true если значение изменилось</returns>
		protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (Equals(field, value)) return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}
	}
}