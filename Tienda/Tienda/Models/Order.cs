using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tienda.Models
{
    [Table("Orders", Schema = "dbo")]
    public class Order
    {
        [Key]
        [Column("Id", TypeName = "int")]
        public int Id { get; set; }

        [Display(Prompt = "Nombre del cliente", Name = "Nombre")]
        [Required(ErrorMessage = "El nombre del cliente es requerido")]
        [StringLength(80)]
        [Column("Customer_name", TypeName = "varchar(80)")]
        public string CustomerName { get; set; }

        [Display(Prompt = "Documento del cliente", Name = "Documento")]
        [Required(ErrorMessage = "El documento del cliente es requerido")]
        [StringLength(20)]
        [Column("Customer_Document", TypeName = "varchar(20)")]
        public string CustomerDocument { get; set; }

        [Display(Prompt = "Email del cliente", Name = "Email")]
        [Required(ErrorMessage = "El email del cliente es requerido")]
        [StringLength(120)]
        [Column("Customer_email", TypeName = "varchar(120)")]        
        [EmailAddress(ErrorMessage = "Por favor ingrese un email válido")]
        public string CustomerEmail { get; set; }

        [Display(Prompt = "Celualar del cliente", Name = "Celular")]
        [Required(ErrorMessage = "El celular del cliente es requerido")]
        [StringLength(40)]
        [Column("Customer_mobile", TypeName = "varchar(40)")]
        public string CustomerMobile { get; set; }

        [Display(Name = "Estado")]
        [StringLength(20)]
        [Column("Status", TypeName = "varchar(20)")]
        public string Status { get; set; }

        [Display(Name = "Fecha")]
        [Column("Created_at", TypeName = "datetime2")]       
        public DateTime CreatedAt { get; set; }

        [Column("Updated_at", TypeName = "datetime2")]
        public DateTime UpdatedAt { get; set; }

        [Column("ValorOrder", TypeName = "money")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c2}")]
        public double ValorOrder { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
    }
}
