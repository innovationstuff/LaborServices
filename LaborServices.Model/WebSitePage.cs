using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaborServices.Model
{
    public class WebSitePage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PageId { get; set; }

        public byte PageName { get; set; }

        [Column(TypeName = "nvarchar"), MaxLength(300)]
        [Required]
        public string TitleEn { get; set; }

        [Column(TypeName = "nvarchar"), MaxLength(300)]
        [Required]
        public string TitleAr { get; set; }

        public string DescriptionEn { get; set; }

        public string DescriptionAr { get; set; }

        [Column(TypeName = "nvarchar"), MaxLength(100)]
        public string Slug { get; set; }
    }
}
