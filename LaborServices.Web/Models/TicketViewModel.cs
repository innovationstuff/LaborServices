using LaborServices.ViewModel.Resources;
using System.ComponentModel.DataAnnotations;

namespace LaborServices.Web.Models
{
    public class CustomerTicket
    {
        public string ClientClosedCode { get; set; }
        public string TicketNumber { get; set; }

        [Required(ErrorMessageResourceName = "ProblemDetails", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Display(Name = "Details")]
        public string ProblemDetails { get; set; }
        public string Id { get; set; }

        [Required(ErrorMessageResourceName = "ContactId", ErrorMessageResourceType = typeof(ValidationsResources))]
        public string ContactId { get; set; }
        public string Contact { get; set; }

        [Required(ErrorMessageResourceName = "ProblemTypeId", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Display(Name = "Problem Type")]
        public string ProblemTypeId { get; set; }
        public string ProblemType { get; set; }

        public string SectorTypeId { get; set; }
        public string SectorType { get; set; }

        public string StatusId { get; set; }
        public string Status { get; set; }

        [Required(ErrorMessageResourceName = "ContractId", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Display(Name = "Contract")]
        public string ContractId { get; set; }
        public string Contract { get; set; }

        [Display(Name = "Worker")]
        public string EmployeeId { get; set; }
        public string EmployeeText { get; set; }

        public int Who => 2;
    }
}