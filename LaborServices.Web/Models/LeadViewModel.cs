using LaborServices.Utility;
using LaborServices.ViewModel.Resources;
using LaborServices.ViewModel.Resources.WestWindResources;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LaborServices.Web.Models
{
    public class BusinessLeadViewModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }

        [Required(ErrorMessageResourceName = "CityId", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Display(Name = "المدينة")]
        public string CityId { get; set; }

        [Required(ErrorMessageResourceName = "DistrictId", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Display(Name = "المنطقة")]
        public string RegionName { get; set; }

        [Required(ErrorMessageResourceName = "CompanyId", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Display(Name = "الشركة")]
        public string CompanyName { get; set; }
        public string SectorId { get; set; }
        public string IndustryCode { get; set; }
        public string SalesPersonName { get; set; }

        [Required(ErrorMessageResourceName = "RegisterPhoneIsRequired", ErrorMessageResourceType = typeof(ValidationsResources))]
        public string Mobile { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "Jobs", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Display(Name = "المهن المطلوبة")]
        public string Description { get; set; }
        public string Address { get; set; }
        public string Job { get; set; }
    }

    public class RequiredJobsViewModel
    {
        public string Id { get; set; }
        public string Job { get; set; }
        public string Nationality { get; set; }
        public int? EmpsCount { get; set; }
        public decimal? Salary { get; set; }
    }
     
    public class IndividualLeadViewModel
    {
        public IndividualLeadViewModel()
        {
            SectorId = ((byte)SectorsTypeEnum.Individuals).ToString();
        }

        public IndividualLeadViewModel(ContactViewModel contact) : base()
        {
            Name = contact.FullName;
            IdNumber = contact.IdNumber;
            Mobile = contact.MobilePhone;
            CityId = contact.CityId;
            JobTitle = contact.JobTitle;
            RegionId = contact.RegionId;

            Email = contact.Email;
            GenderId = contact.GenderId;
            NationalityId = contact.NationalityId;

            ContactId = contact.ContactId;
        }

        public string Id { get; set; }
        [Required]
        public string RequiredNationality { get; set; }
        public string RequiredNationalityName { get; set; }
        [Required]
        public string RequiredProfession { get; set; }
        public string RequiredProfessionName { get; set; }
        [Required]
        public string CityId { get; set; }
        public string CityName { get; set; }
        //[Required]
        public string RegionId { get; set; }
        public string RegionName { get; set; }
        public string DistrictId { get; set; }
        public string DistrictName { get; set; }
        [Required]
        [Display(Name= "Name", ResourceType = typeof(ProfileResources))]
        public string Name { get; set; }

        [Required]
        [Display(Name= "Mobile", ResourceType = typeof(ProfileResources))]
        public string Mobile { get; set; }

        [Display(Name= "JobTitle", ResourceType = typeof(ProfileResources))]
        public string JobTitle { get; set; }

        public string HomePhone { get; set; }

        [Required]
        [Display(Name= "IdNumber", ResourceType = typeof(ProfileResources))]
        public long? IdNumber { get; set; }

        public bool? IsMedicalLead { get; set; }
        public string SectorId { get; set; }
        public string SalesPersonName { get; set; }
        public string SalesPersonId { get; set; }

        [Display(Name= "Description", ResourceType = typeof(Shared))]
        public string Description { get; set; }


        [Required]
        [Display(Name= "Email", ResourceType = typeof(ProfileResources))]
        public string Email { get; set; }

        [Required]
        public string NationalityId { get; set; }

        [Required]
        public int? GenderId { get; set; }


        public string ContactId { get; set; }


    }

}