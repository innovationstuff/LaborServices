using LaborServices.Utility;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaborServices.Model
{
    public class Setting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SettingId { get; set; }

        [Column(TypeName = "nvarchar"), MaxLength(300)]
        [Display(Name="Setting Name")]
        [Required]
        public string SettingName { get; set; }

        [Display(Name = "Setting Description")]
        public string SettingDescription { get; set; }

        [Display(Name = "Setting DataType")]
        [Required]
        public DataTypes SettingDataType { get; set; }

        [Display(Name = "Setting Value")]
        [Required]
        public string SettingValue { get; set; }

        public bool IsEditable { get; set; }
    }
}
