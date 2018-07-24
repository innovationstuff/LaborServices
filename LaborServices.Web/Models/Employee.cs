using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LaborServices.Web.Models
{
    public class Employee
    {
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string JobTitle { get; set; }
        public string JobNumber { get; set; }
        public string Nationality { get; set; }
        public string Region { get; set; }
        public string Skills { get; set; }
        public string Image { get; set; }
        public string IdNumber { get; set; }
    }
}