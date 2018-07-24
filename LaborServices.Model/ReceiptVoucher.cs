using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborServices.Model
{
    public class ReceiptVoucher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        [Column(TypeName = "nvarchar"), MaxLength(250)]
        public string CustomerId { get; set; }

        [Column(TypeName = "int")]
        public int PaymentType { get; set; }

        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        [Column(TypeName = "nvarchar"), MaxLength(250)]
        public string Date { get; set; }

        [Column(TypeName = "money")]
        public decimal VatRate { get; set; }

        [Column(TypeName = "nvarchar"), MaxLength(250)]
        public string ContractId { get; set; }

        [Column(TypeName = "nvarchar"), MaxLength(250)]
        public string ContractNumber { get; set; }

        [Column(TypeName = "nvarchar"), MaxLength(250)]
        public string PaymentCode { get; set; }

        [Column(TypeName = "int")]
        public int Who { get; set; }

        [Column(TypeName = "bigint")]
        public Int64 TransactionId { get; set; }

        [Column(TypeName = "nvarchar"), MaxLength(250)]
        public string CreatedDate { get; set; }

        [Column(TypeName = "nvarchar"), MaxLength(250)]
        public string ModifiedDate { get; set; }

        [Column(TypeName = "bit")]
        public bool IsSaved { get; set; }
    }
}
