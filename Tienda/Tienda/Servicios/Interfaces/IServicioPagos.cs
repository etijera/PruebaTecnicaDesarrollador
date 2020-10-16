using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tienda.Models;

namespace Tienda.Servicios.Interfaces
{
    public interface IServicioPagos
    {
        Task<IEnumerable<Payment>> ObtenerPagos();
        Task<Payment> ObtenerPago(int id);
        Task<int> CrearPago(Payment pay);
        Task<int> ActualizarPago(Payment pay);

    }
}
