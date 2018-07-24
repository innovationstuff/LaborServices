using LaborServices.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LaborServices.Models
{
    public class PaymentTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        [Column(TypeName = "nvarchar"), MaxLength(250)]
        public string CustomerId { get; set; }

        [Column(TypeName = "nvarchar"), MaxLength(250)]
        public string ContractId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar"), MaxLength(500)]
        public string EntityName { get; set; }

        [Column(TypeName = "nvarchar"), MaxLength(250)]
        public string PaymentStatus { get; set; }

        [Column(TypeName = "nvarchar"), MaxLength(500)]
        public string PaymentStatusName { get; set; }

        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        [Column(TypeName = "int")]
        public int Who { get; set; }

        [Column(TypeName = "bit")]
        public bool IsVoucherSaved { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }

        public int TransactionType { get; set; }
        public string TransactionTypeName { get; set; }
    }
}