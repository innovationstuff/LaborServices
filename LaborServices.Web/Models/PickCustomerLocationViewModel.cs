using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LaborServices.Web.Models
{
    public class PickCustomerLocationViewModel
    {
        public string ContractId { get; set; }
        [Required]
        public string Latitude { get; set; }
        [Required]
        public string Longitude { get; set; }

        //[Required]
        public string HouseType { get; set; }
        //[Required]
        public string HouseNo { get; set; }
        //[Required]
        public string FloorNo { get; set; }
        public string AddressNotes { get; set; }

    }
}