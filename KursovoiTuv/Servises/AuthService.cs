using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using KursovoiTuv.Data;
using KursovoiTuv.Models;

namespace KursovoiTuv.Services
{
	/// <summary>
	/// Сервис аутентификации пользователей
	/// </summary>
	/// <remarks>
	/// Отвечает за проверку учетных данных пользователей
	/// и управление сессиями
	/// </remarks>
	public class AuthService : IAuthService
	{
		private readonly ApplicationDbContext _context;

		/// <summary>
		/// Конструктор сервиса аутентификации
		/// </summary>
		/// <param name="context">Контекст базы данных</param>
		public AuthService(ApplicationDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Аутентифицировать пользователя
		/// </summary>
		/// <param name="username">Имя пользователя</param>
		/// <param name="password">Пароль</param>
		/// <returns>Аутентифицированный пользователь или null</returns>
		public User Authenticate(string username, string password)
		{
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
				return null;

			var passwordHash = ComputeSha256Hash(password);

			return _context.Users
				.FirstOrDefault(u => u.Username == username && u.PasswordHash == passwordHash);
		}

		/// <summary>
		/// Вычислить SHA256 хэш строки
		/// </summary>
		/// <param name="rawData">Исходная строка</param>
		/// <returns>Хэш строки в верхнем регистре</returns>
		private string ComputeSha256Hash(string rawData)
		{
			using (SHA256 sha256Hash = SHA256.Create())
			{
				byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < bytes.Length; i++)
				{
					builder.Append(bytes[i].ToString("x2"));
				}
				return builder.ToString().ToUpper();
			}
		}

		/// <summary>
		/// Проверить, существует ли пользователь
		/// </summary>
		public bool UserExists(string username)
		{
			return _context.Users.Any(u => u.Username == username);
		}
	}
}