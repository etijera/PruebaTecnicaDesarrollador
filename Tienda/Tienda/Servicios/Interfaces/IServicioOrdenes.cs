using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tienda.Models;

namespace Tienda.Servicios.Interfaces
{
    public interface IServicioOrdenes
    {
        Task<int> CrearOrden(Order order);
        Task<Order> ObtenerOrden(int id);
        Task<IEnumerable<Order>> ObtenerOrdenes();
        Task<int> ActualizarOrden(Order order);
        Task<int> CrearOrdenDetalle(OrderDetail orderDetails);
    }
}
