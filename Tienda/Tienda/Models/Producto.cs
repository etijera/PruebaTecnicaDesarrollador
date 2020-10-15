using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tienda.Models
{
    public class Producto
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
        public decimal Valor { get; set; }
    }
}
