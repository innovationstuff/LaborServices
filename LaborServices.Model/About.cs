using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborServices.Model
{
	public class About
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Column(TypeName = "nvarchar"), MaxLength(100)]
		public string TitleAr { get; set; }

		[Column(TypeName = "nvarchar")]
		public string DescriptionAr { get; set; }

		
		[Column(TypeName = "nvarchar"), MaxLength(100)]
		public string TitleEN { get; set; }

		[Column(TypeName = "nvarchar")]
		public string DescriptionEN { get; set; }

		public int Type { get; set; }

		[Column(TypeName = "nvarchar")]
		public string ImgUrl { get; set; }
	}
}
