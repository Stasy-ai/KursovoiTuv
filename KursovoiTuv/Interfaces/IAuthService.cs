using KursovoiTuv.Models;

namespace KursovoiTuv.Services
{
	/// <summary>
	/// Интерфейс сервиса аутентификации
	/// </summary>
	public interface IAuthService
	{
		User Authenticate(string username, string password);
		bool UserExists(string username);
	}
}