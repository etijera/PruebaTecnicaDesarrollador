using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tienda.Models;
using Tienda.Servicios.Interfaces;

namespace Tienda.Servicios
{
    public class ServicioPagos: IServicioPagos
    {
        #region Atributos
        
        private readonly ApplicationDbContext _context;

        #endregion

        #region Constructor
        
        public ServicioPagos(ApplicationDbContext context)
        {
            this._context = context;
        }

        #endregion

        #region Métodos

        public async Task<IEnumerable<Payment>> ObtenerPagos()
        {
            return await this._context.Payments.ToListAsync();
        }

        public async Task<Payment> ObtenerPago(int id)
        {
            return await this._context.Payments.SingleOrDefaultAsync(m => m.PaymentId == id);
        }

        // RETORNA 0 SI NO SE HA EJECUTADO LA ACCIÓN O SI HA HABIDO UN ERROR
        public async Task<int> CrearPago(Payment pay)
        {
            var _result = 0;
            this._context.Payments.Add(pay);
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
        public async Task<int> ActualizarPago(Payment pay)
        {
            var _result = 0;
            this._context.Entry(pay).State = EntityState.Modified;
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

            if (!await PagoExists(pay.PaymentId))
            {
                _result = 0;
            }

            return _result;
        }

        private async Task<bool> PagoExists(int id)
        {
            return await this._context.Payments.AnyAsync(e => e.PaymentId == id);
        }

        #endregion
    }
}
