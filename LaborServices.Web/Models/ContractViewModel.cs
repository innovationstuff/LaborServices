using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LaborServices.Utility;

namespace LaborServices.Web.Models
{

    public class HourlyPricing
    {
        public string HourePrice { get; set; }
        public string HourlypricingId { get; set; }
        public string Name { get; set; }
        public string VisitCount { get; set; }
        public string VisitPrice { get; set; }
        public string Discount { get; set; }
        public string Hours { get; set; }
        public string NoOfMonths { get; set; }
        public string TotalVisit { get; set; }
        public string TotalPrice { get; set; }
        public string MonthVisits { get; set; }
        public string Shift { get; set; }
        public string NationalityId { get; set; }
        public string NationalityName { get; set; }
        public string VersionNumber { get; set; }
        public bool IsAvailable { get; set; }
        public string MonthelyPrice { get; set; }
        public string TotalbeforeDiscount { get; set; }
        public int DayShift { get; set; }
        public string VatRate { get; set; }
        public string VatAmount { get; set; }
        public string TotalPriceWithVat { get; set; }
    }

    public class HourlyPricingCost
    {
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal HourRate { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal TotalPriceBeforeDiscount { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal MonthelyPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal Discount { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal DiscountAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal TotalPriceAfterDiscount { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal VatRate { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal VatAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal TotalPriceWithVat { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal NetPrice { get; set; }
    }

    public class Status
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class HourlyAppointment
    {
        public string Id { get; set; }
        public string StatusCode { get; set; }
        public string ContractId { get; set; }
        public string Status { get; set; }
        public string StatusName { get; set; }
        public string Notes { get; set; }
        public string EmpId { get; set; }
        public string EmpName { get; set; }
        public string ShiftStart { get; set; }
        public string ShiftEnd { get; set; }
        public string ActualShiftStart { get; set; }
        public string ActualShiftEnd { get; set; }
        public int? Rate { get; set; }
        public string CarName { get; set; }
        public string CarId { get; set; }
    }

    public class ServiceContractPerHour
    {
        public string CustomerMobilePhone { get; set; }
        public string ContractId { get; set; }
        [Display(Name = "Contract Num")]
        public string ContractNum { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedOnText { get; set; }
        [Display(Name = "Lat.")]
        public string Latitude { get; set; }
        [Display(Name = "Long.")]
        public string Longitude { get; set; }
        public int? NumOfVisits { get; set; }
        public int? NumOfHours { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Start Day")]
        public DateTime? StartDay { get; set; }
        public int? ContractDuration { get; set; }  // no of weeks
        public string ContractDurationName { get; set; }  // no of weeks
        public string SelectedDays { get; set; } //SelectedDays
        public int? NumOfWorkers { get; set; }
        public DayShifts? Shift { get; set; }
        [Display(Name = "Shift")]
        public string ShiftName { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}")]
        [Display(Name = "Final Price")]
        public decimal? FinalPrice { get; set; }
        public int? UserRate { get; set; }
        public string NationalityId { get; set; }
        public string CityId { get; set; }
        public string DistrictId { get; set; }
        public string CustomerId { get; set; }
        public string HourlyPricingId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Next Appointment")]
        public DateTime? NextAppointment { get; set; }
        [Display(Name = "Hourly Pricing")]
        public string HourlyPricingName { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }

        public string City { get; set; }
        public string Nationality { get; set; }
        public string District { get; set; }
        public string Customer { get; set; }
        public HourlyPricing HourlyPricing { get; set; }
        public Status Status { get; set; }
        public List<HourlyAppointment> HourlyAppointments { get; set; }
        public HourlyPricingCost HourlyPricingCost { get; set; }

        public string PriceBeforeDiscount { get; set; }

        public string totalpricewithoutvat { get; set; }
        public string vatrate { get; set; }
        public string vatamount { get; set; }

    }
}