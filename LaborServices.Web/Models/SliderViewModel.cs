using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Web;
using LaborServices.Model;
using LaborServices.Utility;
using LaborServices.Web.Helpers;

namespace LaborServices.Web.Models
{
    public class SliderViewModel
    {
        public Slider Slider { get; set; }

        [Required]
        [Display(Name = "Image")]
        public HttpPostedFileBase SliderImage { get; set; }

        public string SliderImageUrl =>
            Slider != null && string.IsNullOrEmpty(Slider.ImageName) == false ?
            string.Format("{0}{1}", AppConstants.SliderFolder, Slider.ImageName) : "";


        public bool IsImageExist => string.IsNullOrEmpty(SliderImageUrl) == false && File.Exists(HttpContext.Current.Server.MapPath(SliderImageUrl));
    }
}
