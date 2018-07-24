using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LaborServices.Web.Models
{
    public class AvailableNumber
    {
        public string nationalityId { get; set; }
        public string nationality { get; set; }
        public List<professionCount> professionCounts { get; set; }
    }
    public class professionCount
    {
        public string professionId { get; set; }
        public string profession { get; set; }
        public string count { get; set; }
    }

    public class IndivPricing
    {
        public string Id { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string TypeId { get; set; }
        public string TypeName { get; set; }
        public string NationalityName { get; set; }
        public string ContractMonths { get; set; }
        public string PeriodAmount { get; set; }
        public string EveryMonth { get; set; }
        public string MonthelyPaid { get; set; }
        public string PrePaid { get; set; }
        public string NationalityId { get; set; }
        public string ProfessionId { get; set; }
    }
}