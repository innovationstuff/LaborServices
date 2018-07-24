using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborServices.Model
{
	[Table("Teams")]
	public class Team
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		[Column(TypeName ="nvarchar"),MaxLength(100)]
		public string NameAr { get; set; }

		[Required]
		[Column(TypeName = "nvarchar"), MaxLength(100)]
		public string NameEn { get; set; }

		[Required]
		[Column(TypeName = "nvarchar")]
		public string DescriptionAr { get; set; }

		[Required]
		[Column(TypeName = "nvarchar"), MaxLength(100)]
		public string DescriptionEn { get; set; }
	}
}
