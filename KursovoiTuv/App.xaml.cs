using System.Windows;
using KursovoiTuv.Data;
using KursovoiTuv.Models;
using KursovoiTuv.Patterns.Facade;
using KursovoiTuv.Services;
using KursovoiTuv.ViewModel;
using KursovoiTuv.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KursovoiTuv
{
	public partial class App : Application
	{
		public static User CurrentUser { get; set; }

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			//// Создаем БД при запуске
			//using (var dbContext = new ApplicationDbContext())
			//{
			//	dbContext.Database.EnsureCreated();
			//}

			//// Показываем главное окно БЕЗ DI
			//var loginWindow = new LoginWindow();
			//loginWindow.Show();
		}


	}
}