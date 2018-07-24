using LaborServices.Utility;
using LaborServices.ViewModel.Resources;
using System.ComponentModel.DataAnnotations;

namespace LaborServices.Web.Models
{
    public class ContactViewModel
    {
        public ContactViewModel()
        {
            
        }
        //  [Required]
        [Required(ErrorMessageResourceName = "ContactId", ErrorMessageResourceType = typeof(ValidationsResources))]
        public string ContactId { get; set; }

        // [Required]
        [Required(ErrorMessageResourceName = "RegisterPhoneIsRequired", ErrorMessageResourceType = typeof(ValidationsResources))]

        public string MobilePhone { get; set; }

        //[Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(ValidationsResources))]

        public string Email { get; set; }

        //[Required(ErrorMessageResourceName = "FullName", ErrorMessageResourceType = typeof(ValidationsResources))]

        public string FullName { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        //public string LastName { get; set; }

        //   [Required]
      //  [Required(ErrorMessageResourceName = "JobTitle", ErrorMessageResourceType = typeof(ValidationsResources))]

        public string JobTitle { get; set; }

        //  [Required]
        [Required(ErrorMessageResourceName = "CityId", ErrorMessageResourceType = typeof(ValidationsResources))]

        public string CityId { get; set; }

        // [Required]
        [Required(ErrorMessageResourceName = "NationalityId", ErrorMessageResourceType = typeof(ValidationsResources))]

        public string NationalityId { get; set; }

        // [Required]
        [Required(ErrorMessageResourceName = "GenderId", ErrorMessageResourceType = typeof(ValidationsResources))]

        public int? GenderId { get; set; }

        //  [Required]
        [Required(ErrorMessageResourceName = "IdNumber", ErrorMessageResourceType = typeof(ValidationsResources))]

        [RegularExpression(AppConstants.IdNumberRex, ErrorMessageResourceName = "NotValidIqama", ErrorMessageResourceType = typeof(ValidationsResources))]
        public long? IdNumber { get; set; }

        //   [Required]
        //[Required(ErrorMessageResourceName = "RegionId", ErrorMessageResourceType = typeof(ValidationsResources))]

        public string RegionId { get; set; }
    }
}