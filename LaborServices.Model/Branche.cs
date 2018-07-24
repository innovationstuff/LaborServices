using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborServices.Model
{
	[Table("Branches")]
	public class Branche
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }


		[Column(TypeName = "nvarchar"), MaxLength(100)]
		public string NameAr { get; set; }

		[Column(TypeName = "nvarchar")]
		public string LocationAr { get; set; }

		[Column(TypeName = "nvarchar"), MaxLength(100)]
		public string NameEn { get; set; }

		[Column(TypeName = "nvarchar")]
		public string LocationEn { get; set; }

		[Column(TypeName = "nvarchar")]
		public string PhoneNumber { get; set; }

		[Column(TypeName = "nvarchar")]
		public string MapLink { get; set; }

		//Social Links

		[Column(TypeName = "nvarchar")]
		public string FaceBookLink { get; set; }

		[Column(TypeName = "nvarchar")]
		public string InstagramLink { get; set; }

		[Column(TypeName = "nvarchar")]
		public string TwitterLink { get; set; }
	}
}
