using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace KursovoiTuv.ViewModel
{
    class StatusToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string status)
			{
				return status switch
				{
					"Новый" => new SolidColorBrush(Color.FromRgb(52, 152, 219)),      // Синий
					"В работе" => new SolidColorBrush(Color.FromRgb(243, 156, 18)),    // Оранжевый
					"На проверке" => new SolidColorBrush(Color.FromRgb(155, 89, 182)), // Фиолетовый
					"Завершен" => new SolidColorBrush(Color.FromRgb(39, 174, 96)),     // Зеленый
					_ => new SolidColorBrush(Color.FromRgb(149, 165, 166))             // Серый
				};
			}
			return new SolidColorBrush(Color.FromRgb(149, 165, 166));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
