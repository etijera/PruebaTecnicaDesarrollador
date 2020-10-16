using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tienda.Models;
using Tienda.Servicios;

namespace TiendaTest.Servicios
{
    [TestClass]
    public class ServicioPagosTest
    {
        // Clase Base de Datos Fake
        private BasePruebas sqDbEnMemoria;

        [TestInitialize]
        public void Init()
        {
            sqDbEnMemoria = new BasePruebas();
        }

        [TestMethod]
        public void CrearPagoValidaEnDbDevuelveNumerosRegistrosInsertadosAsync()
        {
            // Creamos un DbContext limpio para hacer las pruebas
            using (var context = sqDbEnMemoria.ConstruirContext("Base21"))
            {
                // ARRANGE
                // Creamos un objeto orden válido
                var ordenTest = new Order()
                {
                    CustomerName = "Cliente1",
                    CustomerDocument = "123456",
                    CustomerEmail = "miemail@mio.com",
                    CustomerMobile = "310256584",
                    Status = "CREATED",
                    CreatedAt = DateTime.Now,
                    ValorOrder = Convert.ToDouble(150000)
                };

                context.Orders.Add(ordenTest);
                context.SaveChanges();

                var order = context.Orders.Single();

                var orderDetail = new OrderDetail()
                {
                    OrderId = order.Id,
                    CodigoProducto = "01",
                    NombreProducto = "Producto 01",
                    Cantidad = 1,
                    Valor = 20000,
                    Total = 20000
                };

                context.OrderDetails.Add(orderDetail);
                context.SaveChanges();

                var pagoTest = new Payment()
                {
                    OrderId = order.Id,
                    Fecha = DateTime.Now,
                    RequestId = 123,
                    Status = "OK",
                    UrlPago = "https://www.pagame.com"
                };
            
                // ACT
                var servicioPago = new ServicioPagos(context);
                // Invocamos el Método que estamos probando 'CrearPago(Payment pago)'
                var result = servicioPago.CrearPago(pagoTest);

                // ASSERT
                // Aseguramos que el resultado del método 'CrearPago(pagoTest)' es distinto de 0.
                Assert.AreNotEqual(0, result.Result);
            }
        }
        
        [TestMethod]
        public void ActualizarPagoModificarPagoExistenteEnBbDevuelveNumeroDeRegistrosModificadosAsync()
        {
            using (var context = sqDbEnMemoria.ConstruirContext("Base22"))
            {
                // ARRANGE
                var ordenTest = new Order()
                {
                    CustomerName = "Cliente1",
                    CustomerDocument = "123456",
                    CustomerEmail = "miemail@mio.com",
                    CustomerMobile = "310256584",
                    Status = "CREATED",
                    CreatedAt = DateTime.Now,
                    ValorOrder = Convert.ToDouble(150000)
                };

                context.Orders.Add(ordenTest);
                context.SaveChanges();

                var order = context.Orders.Single();

                var orderDetailTest = new OrderDetail()
                {
                    OrderId = order.Id,
                    CodigoProducto = "01",
                    NombreProducto = "Producto 01",
                    Cantidad = 1,
                    Valor = 20000,
                    Total = 20000
                };

                context.OrderDetails.Add(orderDetailTest);
                context.SaveChanges();

                var pagoTest = new Payment()
                {
                    OrderId = order.Id,
                    Fecha = DateTime.Now,
                    RequestId = 123,
                    Status = "OK",
                    UrlPago = "https://www.pagame.com"
                };

                context.Payments.Add(pagoTest);
                context.SaveChanges();

                // ACT
                var pago = context.Payments.Single();
                // Desconectamos el Objeto 'pago' del Contexto para poder modificarlo posteriormente.
                context.Entry(pago).State = EntityState.Detached;

                var pagoModificado = new Payment() { PaymentId = pago.PaymentId, OrderId = pago.OrderId, Fecha = DateTime.Now, RequestId = 123, Status = "PAYED", FechaUpdate = DateTime.Now, UrlPago = "https://www.pagame.com" };
                var servicioPago = new ServicioPagos(context);
                var result = servicioPago.ActualizarPago(pagoModificado);

                // ASSERT
                Assert.AreNotEqual(0, result.Result);
                Assert.AreEqual(1, result.Result);
                Assert.AreEqual("PAYED", context.Payments.Single().Status);
            }
        }
    }
}
