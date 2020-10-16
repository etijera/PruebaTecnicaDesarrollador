using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tienda.Controllers;
using Tienda.Models;
using Tienda.Servicios;

namespace TiendaTest.Controllers
{
    [TestClass]
    public class OrdersControllerTest
    {
        // Clase Base de Datos Fake
        private BasePruebas sqDbEnMemoria;

        [TestInitialize]
        public void Init()
        {
            sqDbEnMemoria = new BasePruebas();
        }

        [TestMethod]
        public void IndexGetRetornaVistaIndexConTodosLosRegistrosLasOrdenesBDAsync()
        {
            // Arrange
            var context = sqDbEnMemoria.ConstruirContext("Base1");
            var servicioOrdenes = new ServicioOrdenes(context);
            var servicioPago = new ServicioPagos(context);

            string expectedView = "Index";
            var ordersController = new OrdersController(servicioOrdenes, servicioPago);

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

            context.SaveChanges();

            // Act
            var actionResult = ordersController.Index().Result as ViewResult;
            // Assert
            Assert.IsNotNull(actionResult,
                "El ActionResult no debe ser Null.");
            Assert.IsInstanceOfType(actionResult, typeof(ViewResult),
                "El ActionResult debe ser del tipo ViewResult.");
            Assert.AreEqual(actionResult.ViewName, expectedView,
                "El nombre de la Vista devuelta debe ser " + expectedView);
            Assert.IsInstanceOfType(actionResult.Model, typeof(IEnumerable<Order>),
                "El Modelo pasado a la Vista debe ser del tipo IEnumerable<Order>.");
            Assert.AreEqual(2, (actionResult.Model as IEnumerable<Order>).Count(),
                "El Modelo pasado a la Vista contiene todos los registros de la BD.");
        }

        [TestMethod]
        public void DetailsGetRetornaVistaDetailsConRegistroExistenteEnLaBbAsync()
        {
            // Arrange
            var context = sqDbEnMemoria.ConstruirContext("Base2");
            var servicioOrdenes = new ServicioOrdenes(context);
            var servicioPago = new ServicioPagos(context);

            string expectedView = "Details"; 
            var ordersController = new OrdersController(servicioOrdenes, servicioPago);

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

            context.SaveChanges();
            // Act
            var actionResult = ordersController.Details(1,"").Result as ViewResult;
            // Assert
            Assert.IsNotNull(actionResult,
                "El ActionResult no debe ser Null.");
            Assert.IsInstanceOfType(actionResult, typeof(ViewResult),
                "El ActionResult debe ser del tipo ViewResult.");
            Assert.AreEqual(actionResult.ViewName, expectedView,
                "El nombre de la Vista devuelta debe ser " + expectedView);
            Assert.IsInstanceOfType(actionResult.Model, typeof(Order),
                "El Modelo pasado a la Vista debe ser del tipo Order.");
            Assert.AreEqual(1, (actionResult.Model as Order).Id,
                "El Id del Modelo pasado a la Vista es el correcto.");
        }
        
        [TestMethod]
        public void DetailsGetRetornaNotFoundResultAlPasarIdNullAsync()
        {
            // Arrange
            var context = sqDbEnMemoria.ConstruirContext("Base3");
            var servicioOrdenes = new ServicioOrdenes(context);
            var servicioPago = new ServicioPagos(context);

            var ordersController = new OrdersController(servicioOrdenes, servicioPago);

            // Act
            var actionResult = ordersController.Details(null,"").Result as NotFoundResult;
            // Assert
            Assert.IsNotNull(actionResult,
                "El ActionResult no debe ser Null.");
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult),
                "El ActionResult debe ser del tipo NotFoundResult.");
        }
        
        [TestMethod]
        public void DetailsGetRetornaNotFoundResultAlPasarIdInexistenteAsync()
        {
            // Arrange
            var context = sqDbEnMemoria.ConstruirContext("Base4");
            var servicioOrdenes = new ServicioOrdenes(context);
            var servicioPago = new ServicioPagos(context);

            var ordersController = new OrdersController(servicioOrdenes, servicioPago);

            // Act
            var actionResult = ordersController.Details(0,"").Result as NotFoundResult;
            // Assert
            Assert.IsNotNull(actionResult,
                "El ActionResult no debe ser Null.");
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult),
                "El ActionResult debe ser del tipo NotFoundResult.");
        }



        [TestMethod]
        public void CreateGetRetornaVistaCreate()
        {
            // Arrange
            string expectedView = "Create";
            var context = sqDbEnMemoria.ConstruirContext("Base5");
            var servicioOrdenes = new ServicioOrdenes(context);
            var servicioPago = new ServicioPagos(context);

            var ordersController = new OrdersController(servicioOrdenes, servicioPago);
            // Act
            var actionResult = ordersController.Create("01","Producto 01","imagen.jpg","20000") as ViewResult;
            // Assert
            Assert.IsNotNull(actionResult,
                "El ActionResult no debe ser Null.");
            Assert.IsInstanceOfType(actionResult, typeof(ViewResult),
                "El ActionResult debe ser del tipo ViewResult.");
            Assert.AreEqual(actionResult.ViewName, expectedView,
                "El nombre de la Vista devuelta debe ser " + expectedView);
        }

 
        [TestMethod]
        public void CreatePostRetornaVistaCreateConModeloAlPasarModeloInvalidoAsync()
        {
            // Arrange
            var context = sqDbEnMemoria.ConstruirContext("Base6");
            var servicioOrdenes = new ServicioOrdenes(context);
            var servicioPago = new ServicioPagos(context);

            string expectedView = "Create";
            var ordersController = new OrdersController(servicioOrdenes, servicioPago);

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

            context.SaveChanges();

            var order = new Order()
            {
                CustomerDocument = "123",
                CustomerEmail = "cliente1@gmail.com",
                CustomerMobile = "321",
                Status = "CREATED",
                CreatedAt = DateTime.Now,
                ValorOrder = Convert.ToDouble(150000)
            };

            // Añadir el error para que el ModelState.IsValid lo detecte
            // antes de añadir el registro a la BD.
            ordersController.ModelState.AddModelError("CustomerName", "Required");

            // Act
            var actionResult = ordersController.Create(order, "01", "Producto 01", "20000").Result as ViewResult;
            // Assert
            Assert.IsNotNull(actionResult,
                "El ActionResult no debe ser Null.");
            Assert.IsInstanceOfType(actionResult, typeof(ViewResult),
                "El ActionResult debe ser del tipo ViewResult.");
            Assert.AreEqual(actionResult.ViewName, expectedView,
                "El nombre de la Vista devuelta debe ser " + expectedView);
            Assert.AreEqual(order, actionResult.Model,
                "El Modelo pasado a la Vista es el Modelo inválido.");
            Assert.AreEqual(2, servicioOrdenes.ObtenerOrdenes().Result.Count(),
                "No se ha creado el registro en la BD.");
        }
        
        [TestMethod]
        public void CreatePostRetornaVistaDetailsalCrearUnaOrdenValidoEnBbAsync()
        {
            // Arrange
            string expectedView = "Details";

            var context = sqDbEnMemoria.ConstruirContext("Base7");
            var servicioOrdenes = new ServicioOrdenes(context);
            var servicioPago = new ServicioPagos(context);

            context.Orders.Add(new Order()
            {
                CustomerName = "Cliente 1",
                CustomerDocument = "456",
                CustomerEmail = "cliente2@gmail.com",
                CustomerMobile = "654",
                Status = "CREATED",
                CreatedAt = DateTime.Now,
                ValorOrder = Convert.ToDouble(150000)
            });

            context.SaveChanges();

            var ordersController = new OrdersController(servicioOrdenes, servicioPago);

            var order = new Order()
            {
                CustomerName = "Cliente 2",
                CustomerDocument = "123",
                CustomerEmail = "cliente1@gmail.com",
                CustomerMobile = "321",
                Status = "CREATED",
                CreatedAt = DateTime.Now,
                ValorOrder = Convert.ToDouble(150000)
            };
          
            // Act
            var actionResult = ordersController.Create(order, "01", "Producto 01", "20000").Result as RedirectToActionResult;
            // Assert
            Assert.IsNotNull(actionResult,
                "El ActionResult no debe ser Null.");
            Assert.IsInstanceOfType(actionResult, typeof(RedirectToActionResult),
                "El ActionResult debe ser del tipo RedirectToActionResult.");
            Assert.AreEqual(actionResult.ActionName, expectedView,
                "El nombre de la Vista devuelta debe ser " + expectedView);
            Assert.AreEqual(2, servicioOrdenes.ObtenerOrdenes().Result.Count(),
                "Se ha creado un nuevo registro en la BD.");
            Assert.AreEqual(order.CustomerName, servicioOrdenes.ObtenerOrden(2).Result.CustomerName,
                "El nombre del nuevo registro creado es el correcto.");
        }
        

    }
}
