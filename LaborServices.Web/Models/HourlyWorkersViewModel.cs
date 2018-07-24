using LaborServices.ViewModel.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LaborServices.Web.Models
{
    public class HourlyWorkersViewModel
    {
       
        public  HourlyWorkersViewModel()
            {

            }

        // [Required]
        [Required(ErrorMessageResourceName = "CityId", ErrorMessageResourceType = typeof(ValidationsResources))]
        [DisplayName("City")]
        //[Display(Name = "CityId", ResourceType = typeof(Resources.DalalResources))]
        public string CityId { get; set; }
        

        [DisplayName("City")]
        public string CityName { get; set; }

        // [Required]
        [Required(ErrorMessageResourceName = "DistrictId", ErrorMessageResourceType = typeof(ValidationsResources))]
        [DisplayName("District")]
        public string DistrictId { get; set; }

        [DisplayName("District")]
        public string DistrictName { get; set; }

        //   [Required]
        [Required(ErrorMessageResourceName = "NumberOfworkers", ErrorMessageResourceType = typeof(ValidationsResources))]
        [DisplayName("Number Of workers")]
        public int NumOfWorkers { get; set; }

        // [Required]
        [Required(ErrorMessageResourceName = "NumberOfvisits", ErrorMessageResourceType = typeof(ValidationsResources))]
        [DisplayName("Number Of visits")]
        public int NumOfVisits { get; set; }


        [DisplayName("Number Of visits ")]
        public string NumOfVisitsWritten { get; set; }

        // [Required]
        [Required(ErrorMessageResourceName = "NumberOfhours", ErrorMessageResourceType = typeof(ValidationsResources))]
        [DisplayName("Number Of hours")]
        public int NumOfHours { get; set; }

        // [Required]
        [Required(ErrorMessageResourceName = "Availabledays", ErrorMessageResourceType = typeof(ValidationsResources))]
        [DisplayName("Available days")]
        public string[] AvailableDays { get; set; }

        [DisplayName("Available days")]
        public string AvailableDaysJoined => AvailableDays == null ? "" : string.Join(",", AvailableDays);

        //[Required]
        [Required(ErrorMessageResourceName = "Startday", ErrorMessageResourceType = typeof(ValidationsResources))]

        [DisplayName("Start day")]
        public string StartDay { get; set; }

        //[Required]
        [Required(ErrorMessageResourceName = "ContractDuration", ErrorMessageResourceType = typeof(ValidationsResources))]

        [DisplayName("Contract duration")]
        public int ContractDuration { get; set; }

        [DisplayName("Contract duration")]
        public string ContractDurationWritten { get; set; }

     //   [Required]
        [Required(ErrorMessageResourceName = "Nationality", ErrorMessageResourceType = typeof(ValidationsResources))]

        public string Nationality { get; set; }

        [DisplayName("Nationality")]
        public string NationalityName { get; set; }

       // [Required]
        [Required(ErrorMessageResourceName = "packages", ErrorMessageResourceType = typeof(ValidationsResources))]

        [DisplayName("packages")]
        public string HourlypricingId { get; set; }

        [DisplayName("package Price")]
        public string TotalPrice { get; set; }

        [DisplayName("package Price after Discount")]
        public string TotalBeforeDiscount { get; set; }

        [DisplayName("Is morning shift")]
        public bool? IsMorningShift { get; set; }

        [DisplayName("Package Type")]
        public string PackageTypeName => IsMorningShift.GetValueOrDefault(false) ? "صباحا" : "مساءا";


        [DisplayName("Agreed To Terms")]
        [Range(typeof(bool), "true", "true", ErrorMessageResourceName = "AgreedToTerms", ErrorMessageResourceType = typeof(ValidationsResources))]
        public bool AgreedToTerms { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public List<string> ConfirmTimeTableList { get; set; }

        [DisplayName("Total Contract Duration")]
        public int TotalContractDuration => NumOfVisits * ContractDuration;

      // [Required]
        [DisplayName("House Type")]
        public string HouseType { get; set; }

       // [Required]
        [DisplayName("House No")]
        public string HouseNo { get; set; }

      // [Required]
        [DisplayName("Floor No")]
        public string FloorNo { get; set; }

        [DisplayName("Partment No")]
        public string PartmentNo { get; set; }

        [DisplayName("Address Notes")]
        public string AddressNotes { get; set; }

        public string Discount { get; set; }
        public string VatRate { get; set; }
        public string VatAmount { get; set; }
        public string TotalPriceWithVat { get; set; }
        public string TotalPromotionDiscountAmount { get; set; }
        public string TotalPriceAfterPromotion { get; set; }
        public int PromotionExtraVisits { get; set; }
        public string PromotionCode { get; set; }
        public string PromotionName { get; set; }

    }

    /// <summary>
    /// used only to map to the fucking api function params
    /// </summary>
    public class MiniHourlyWorkersViewModel
    {
        public string NationalityId { get; set; }

        [DisplayName("Available days")]
        public string Days { get; set; }

        [DisplayName("Start day")]
        public string ContractStartDate { get; set; }

        [DisplayName("Contract duration")]
        public int ContractDuration { get; set; }

        [DisplayName("District")]
        public string DistrictId { get; set; }

        [DisplayName("Number Of visits")]
        public int NoOfVisits { get; set; }

        [DisplayName("Number Of hours")]
        public int HoursCount { get; set; }

        [DisplayName("Number Of workers")]
        public int Empcount { get; set; }

        [DisplayName("Weekly visits")]
        public int Weeklyvisits { get; set; }

        [DisplayName("city")]
        public string CityId { get; set; }

        public string PromotionCode { get; set; }
    }

    /// <summary>
    /// used to create contract
    /// </summary>
    public class ContractViewModel
    {
        public ContractViewModel()
        {

        }

        public ContractViewModel(HourlyWorkersViewModel entity)
        {
            CityId = entity.CityId;
            DistrictId = entity.DistrictId;
            NationalityId = entity.Nationality;
            NumOfVisits = entity.NumOfVisits;
            NumOfHours = entity.NumOfHours;
            NumOfWorkers = entity.NumOfWorkers;
            AvailableDays = string.Join(",", entity.AvailableDays);
            HourlyPricingId = entity.HourlypricingId;
            StartDay = entity.StartDay;
            Latitude = entity.Latitude;
            Longitude = entity.Longitude;
            ContractDuration = entity.ContractDuration;

            HouseNo = entity.HouseNo;
            HouseType = entity.HouseType;
            FloorNo = entity.FloorNo;
            AddressNotes = entity.AddressNotes;
            PartmentNo = entity.PartmentNo;
            PromotionCode = entity.PromotionCode;

        }

        public string ContractId { get; set; }

        public string ContractNum { get; set; }

        [DisplayName("City")]
        public string CityId { get; set; }

        [DisplayName("District")]
        public string DistrictId { get; set; }

        [DisplayName("Nationality")]
        public string NationalityId { get; set; }

        [DisplayName("Number Of visits")]
        public int NumOfVisits { get; set; }

        [DisplayName("Number Of hours")]
        public int NumOfHours { get; set; }

        [DisplayName("Number Of workers")]
        public int NumOfWorkers { get; set; }

        [DisplayName("Available days")]
        public string AvailableDays { get; set; }

        [DisplayName("packages")]
        public string HourlyPricingId { get; set; }

        [DisplayName("Start day")]
        public string StartDay { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public int Who => 2;

        [DisplayName("Contract duration")]
        public int ContractDuration { get; set; }

        [DisplayName("Customer")]
        public string CustomerId { get; set; }

        public string Customer { get; set; }


        public string PriceBeforeDiscount { get; set; }
        public string VatAmount { get; set; }
        public string FinalPrice { get; set; }

        [DisplayName("House Type")]
        public string HouseType { get; set; }

        [DisplayName("House No")]
        public string HouseNo { get; set; }

        [DisplayName("Floor No")]
        public string FloorNo { get; set; }

        [DisplayName("Partment No")]
        public string PartmentNo { get; set; }

        [DisplayName("Address Notes")]
        public string AddressNotes { get; set; }

        public string PromotionCode { get; set; }

        public string VatRate { get; set; }
    }
}