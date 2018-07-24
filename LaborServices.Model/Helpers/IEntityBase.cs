using LaborServices.Entity.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborServices.Entity
{
    public interface IEntityBase
    { 
        int Id { get; set; }
        string Name { get; set; }
        // Status >> How to represent?
        DateTime? CreatedOn { get; set; }
        string CreatedBy { get; set; }
        DateTime? LastModifiedOn { get; set; }
        string LastModifiedBy { get; set; }
        DateTime? DeletedOn { get; set; }
        string DeletedBy { get; set; }
        string OwnerId { get; set; }
        ApplicationUser Owner { get; set; }
        bool IsDeleted { get; set; }
        bool IsActivated { get; set; }
        // TimeStamp
    }
}
