using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KursovoiTuv.Models
{
	/// <summary>
	/// Пользователь системы
	/// </summary>
	/// <remarks>
	/// Хранит учетные данные пользователя и его роль
	/// для авторизации в системе
	/// </remarks>
	public class User
	{
		/// <summary>
		/// Уникальный идентификатор пользователя
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Имя пользователя для входа
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// Хэш пароля пользователя
		/// </summary>
		public string PasswordHash { get; set; }

		/// <summary>
		/// Роль пользователя (Admin/Manager/User)
		/// </summary>
		public string Role { get; set; }
	}
}