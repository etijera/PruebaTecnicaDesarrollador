using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tienda.Models
{

    [Table("OrderDetails", Schema = "dbo")]
    public class OrderDetail
    {

        [Key]
        [Column("OrderDetailsId", TypeName = "int")]
        public int OrderDetailsId { get; set; }

        [Column("OrderId", TypeName = "int")]
        public int OrderId { get; set; }

        [Column("CodigoProducto", TypeName = "varchar(10)")]
        public string CodigoProducto { get; set; }

        [Column("NombreProducto", TypeName = "varchar(200)")]
        public string NombreProducto { get; set; }

        [Column("Cantidad", TypeName = "int")]
        public int Cantidad { get; set; }

        [Column("Valor", TypeName = "money")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c2}")]
        public double Valor { get; set; }

        [Column("Total", TypeName = "money")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c2}")]
        public double Total { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }
    }
}
