using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LaborServices.Web.Models
{
    public class ProfileModel
    {
        public ProfileModel()
        {

        }

        public ContactViewModel ContactViewModel { get; set; }
        public CustomerTicket CustomerTicket { get; set; }
    }
}