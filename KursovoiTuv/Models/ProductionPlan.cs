using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KursovoiTuv.Models
{
	internal class ProductionPlan
	{
		public int Id { get; set; }
		public int OrderId { get; set; }
		public string Machine { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public string Operator { get; set; }
		public int HoursRequired { get; set; }
	}
}
