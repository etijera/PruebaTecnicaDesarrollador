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
    public class ServicioOrdenesTest:BasePruebas
    {
        // Clase Base de Datos Fake
        private BasePruebas sqDbEnMemoria;

        [TestInitialize]
        public void Init()
        {
            sqDbEnMemoria = new BasePruebas();
        }

        #region Métodos

        [TestMethod]
        public void CrearOrdenValidaEnDbDevuelveNumerosRegistrosInsertadosAsync()
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

            // Creamos un DbContext limpio para hacer las pruebas
            using (var context = sqDbEnMemoria.ConstruirContext("Base11"))
            {
                // ACT
                var servicioOrdenes = new ServicioOrdenes(context);
                // Invocamos el Método que estamos probando 'CrearOrden(Order orden)'
                var result = servicioOrdenes.CrearOrden(ordenTest);

                // ASSERT
                // Aseguramos que el resultado del método 'CrearOrden(orden)' es distinto de 0.
                Assert.AreNotEqual(0, result.Result);
            }

        }        

        [TestMethod]
        public void ObtenerOrdenesRetornaTodasLasOrdenesDeLaDbAsync()            
        {
            using (var context = sqDbEnMemoria.ConstruirContext("Base12"))
            {

                // ARRANGE
                // Creamos varias ordenes
                context.Orders.Add(new Order()
                                    {
                                        CustomerName = "Cliente 1",
                                        CustomerDocument = "123",
                                        CustomerEmail = "cliente1@gmail.com",
                                        CustomerMobile = "321",
                                        Status = "CREATED",
                                        CreatedAt = DateTime.Now,
                                        ValorOrder = Convert.ToDouble(150000)
                                    });

                context.Orders.Add(new Order()
                {
                    CustomerName = "Cliente 2",
                    CustomerDocument = "456",
                    CustomerEmail = "cliente2@gmail.com",
                    CustomerMobile = "654",
                    Status = "CREATED",
                    CreatedAt = DateTime.Now,
                    ValorOrder = Convert.ToDouble(150000)
                });

                context.Orders.Add(new Order()
                {
                    CustomerName = "Cliente 3",
                    CustomerDocument = "789",
                    CustomerEmail = "cliente3@gmail.com",
                    CustomerMobile = "987",
                    Status = "CREATED",
                    CreatedAt = DateTime.Now,
                    ValorOrder = Convert.ToDouble(150000)
                });

                context.SaveChanges();

                // ACT
                var servicioOrdenes = new ServicioOrdenes(context);
                var result = servicioOrdenes.ObtenerOrdenes();

                // ASSERT
                Assert.AreEqual(3, result.Result.Count());
            }
        }

        [TestMethod]
        public void ObtenerOrdenRetornarNullSiLaOrdenNoExisteEnBbAsync()
        {
            // Creamos un DbContext limpio para hacer las pruebas
            using (var context = sqDbEnMemoria.ConstruirContext("Base13"))
            {
                // ACT
                var servicioOrdenes = new ServicioOrdenes(context);
                var result = servicioOrdenes.ObtenerOrden(1);

                // ASSERT
                Assert.IsNull(result.Result);
            }
        }

        [TestMethod]
        public void ActualizarOrdenModificarOrdenExistenteEnBbDevuelveNumeroDeRegistrosModificadosAsync()
        {
            using (var context = sqDbEnMemoria.ConstruirContext("Base14"))
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

                // ACT
                var order = context.Orders.Single();
                // Desconectamos el Objeto 'order' del Contexto para poder modificarlo posteriormente.
                context.Entry(order).State = EntityState.Detached;

                var orderModificado = new Order() { Id = order.Id, CustomerName = "Cliente Modificado", CustomerDocument = "123456", CustomerEmail = "miemail@mio.com",};
                var servicioOrdenes = new ServicioOrdenes(context);
                var result = servicioOrdenes.ActualizarOrden(orderModificado);

                // ASSERT
                Assert.AreNotEqual(0, result.Result);
                Assert.AreEqual(1, result.Result);
                Assert.AreEqual("Cliente Modificado", context.Orders.Single().CustomerName);
            }
        }

        [TestMethod]
        public void CrearOrdenDetalleValidaEnDbDevuelveNumerosRegistrosInsertadosAsync()
        {
            using (var context = sqDbEnMemoria.ConstruirContext("Base15"))
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

                // ACT
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
                
                var servicioOrdenes = new ServicioOrdenes(context);
                var result = servicioOrdenes.CrearOrdenDetalle(orderDetail);

                // ASSERT
                Assert.AreNotEqual(0, result.Result);
            }
        }

        #endregion
    }
}
