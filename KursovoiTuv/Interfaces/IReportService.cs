using System;

namespace KursovoiTuv.Services
{
	/// <summary>
	/// Интерфейс сервиса для генерации отчетов
	/// </summary>
	public interface IReportService
	{
		string GenerateReport(string reportType, DateTime startDate, DateTime endDate);
	}
}