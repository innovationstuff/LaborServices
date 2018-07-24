using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaborServices.Model
{
    public class Slider
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar"), MaxLength(100)]
        public string Title { get; set; }

        [Column(TypeName = "nvarchar"), MaxLength(300)]
        public string Description { get; set; }

        [Column(TypeName = "varchar"), MaxLength(255)]
        public string Link { get; set; }

        [Column(TypeName = "varchar"), MaxLength(255)]
        public string ImageName { get; set; }
    }
}
