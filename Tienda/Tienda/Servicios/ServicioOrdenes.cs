using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tienda.Models;
using Tienda.Servicios.Interfaces;

namespace Tienda.Servicios
{
    public class ServicioOrdenes: IServicioOrdenes
    {
        #region Atributos
        
        private readonly ApplicationDbContext _context;

        #endregion

        #region Constructor
        
        public ServicioOrdenes(ApplicationDbContext context)
        {
            this._context = context;
        }

        #endregion

        #region Métodos

        public async Task<IEnumerable<Order>> ObtenerOrdenes()
        {
            return await this._context.Orders.ToListAsync();
        }

        public async Task<Order> ObtenerOrden(int id)
        {            
            return await this._context.Orders.Include("OrderDetails").Include("Payments").SingleOrDefaultAsync(m => m.Id == id);
        }

        // RETORNA 0 SI NO SE HA EJECUTADO LA ACCIÓN O SI HA HABIDO UN ERROR
        public async Task<int> CrearOrden(Order orden)
        {
            var _result = 0;
            this._context.Orders.Add(orden);
            try
            {
                _result = await this._context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                _result = 0;
            }
            catch (DbUpdateException)
            {
                _result = 0;
            }

            return _result;
        }

        // RETORNA 0 SI NO SE HA EJECUTADO LA ACCIÓN O SI HA HABIDO UN ERROR
        public async Task<int> ActualizarOrden(Order orden)
        {
            var _result = 0;
            this._context.Entry(orden).State = EntityState.Modified;
            try
            {
                _result = await this._context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _result = 0;
            }
            catch (DbUpdateException ex)
            {
                _result = 0;
            }

            if (!await OrdenExists(orden.Id))
            {
                _result = 0;
            }

            return _result;
        }

        // RETORNA 0 SI NO SE HA EJECUTADO LA ACCIÓN O SI HA HABIDO UN ERROR
        public async Task<int> CrearOrdenDetalle(OrderDetail ordenDetails)
        {
            var _result = 0;
            this._context.OrderDetails.Add(ordenDetails);
            try
            {
                _result = await this._context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                _result = 0;
            }
            catch (DbUpdateException)
            {
                _result = 0;
            }

            return _result;
        }

        private async Task<bool> OrdenExists(int id)
        {
            return await this._context.Orders.AnyAsync(e => e.Id == id);
        }

        #endregion

    }
}
