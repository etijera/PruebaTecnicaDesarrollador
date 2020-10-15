using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tienda.Models
{
    [Table("Payment", Schema = "dbo")]
    public class Payment
    {
        [Key]
        [Column("PaymentId", TypeName = "int")]
        public int PaymentId { get; set; }

        [Column("OrderId", TypeName = "int")]
        public int OrderId { get; set; }

        [Column("Fecha", TypeName = "datetime2")]        
        public DateTime Fecha { get; set; }

        [StringLength(20)]
        [Column("Status", TypeName = "varchar(20)")]
        public string Status { get; set; }

        [StringLength(20)]
        [Column("Reason", TypeName = "varchar(20)")]
        public string Reason { get; set; }

        [Column("Message", TypeName = "varchar(max)")]
        public string Message { get; set; }

        [Column("FechaUpdate", TypeName = "datetime2")]
        public DateTime FechaUpdate { get; set; }

        [Column("RequestId", TypeName = "int")]
        public int RequestId { get; set; }

        [Column("UrlPago", TypeName = "varchar(max)")]
        public string UrlPago { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

    }
}
