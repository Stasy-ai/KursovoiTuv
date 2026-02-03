using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using KursovoiTuv.Data;
using KursovoiTuv.Patterns.Facade;
using KursovoiTuv.Services;

namespace KursovoiTuv.ViewModel
{
	public class ReportsViewModel : ObservableObject
	{
		private DateTime _startDate;
		private DateTime _endDate;
		private string _selectedReportType;
		private string _reportText;
		private readonly IReportService _reportService;

		public ReportsViewModel()
		{
			// ВАЖНО: Инициализируем поля ДО создания команд!
			StartDate = DateTime.Now.AddMonths(-1);
			EndDate = DateTime.Now;

			ReportTypes = new ObservableCollection<string>
			{
				"Все заказы",
				"Заказы по статусу",
				"Заказы по приоритету",
				"Просроченные заказы",
				"Финансовый отчет",
				"Активные заказы"
			};

			SelectedReportType = ReportTypes[0];

			// СОЗДАЕМ сервис отчетов
			try
			{
				var dbContext = new ApplicationDbContext();
				var facade = new OrderDatabaseFacade(dbContext);
				_reportService = new ReportService(facade);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка создания сервиса отчетов: {ex.Message}", "Ошибка");
				_reportService = null;
			}

			// Создаем команды
			GenerateCommand = new RelayCommand(GenerateReport);
			ExportCommand = new RelayCommand(ExportToTxt, _ => !string.IsNullOrEmpty(ReportText));
			CloseCommand = new RelayCommand(CloseWindow);
		}

		public DateTime StartDate
		{
			get => _startDate;
			set => SetProperty(ref _startDate, value);
		}

		public DateTime EndDate
		{
			get => _endDate;
			set => SetProperty(ref _endDate, value);
		}

		public ObservableCollection<string> ReportTypes { get; }

		public string SelectedReportType
		{
			get => _selectedReportType;
			set => SetProperty(ref _selectedReportType, value);
		}

		public string ReportText
		{
			get => _reportText;
			private set => SetProperty(ref _reportText, value);
		}

		// Команды
		public ICommand GenerateCommand { get; }
		public ICommand ExportCommand { get; }
		public ICommand CloseCommand { get; }

		public event Action CloseRequested;

		private void GenerateReport(object parameter)
		{
			try
			{
				// ПРОВЕРЯЕМ что сервис не null
				if (_reportService == null)
				{
					MessageBox.Show("Сервис отчетов не инициализирован", "Ошибка");
					return;
				}

				if (StartDate > EndDate)
				{
					MessageBox.Show("Начальная дата не может быть больше конечной", "Ошибка");
					return;
				}

				ReportText = _reportService.GenerateReport(SelectedReportType, StartDate, EndDate);

				if (string.IsNullOrEmpty(ReportText))
				{
					ReportText = "Нет данных для отчета за указанный период.";
				}
			}
			catch (Exception ex)
			{
				ReportText = $"Ошибка при формировании отчета: {ex.Message}";
				MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
			}
		}

		private void ExportToTxt(object parameter)
		{
			try
			{
				if (string.IsNullOrEmpty(ReportText))
				{
					MessageBox.Show("Нет данных для экспорта", "Предупреждение");
					return;
				}

				var saveFileDialog = new Microsoft.Win32.SaveFileDialog
				{
					Filter = "Текстовый файл (*.txt)|*.txt|Все файлы (*.*)|*.*",
					FileName = $"Отчет_{SelectedReportType.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.txt",
					DefaultExt = ".txt"
				};

				if (saveFileDialog.ShowDialog() == true)
				{
					System.IO.File.WriteAllText(saveFileDialog.FileName, ReportText, System.Text.Encoding.UTF8);
					MessageBox.Show($"Отчет сохранен:\n{saveFileDialog.FileName}",
						"Успех", MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка");
			}
		}

		private void CloseWindow(object parameter)
		{
			CloseRequested?.Invoke();
		}
	}
}