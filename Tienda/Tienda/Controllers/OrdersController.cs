using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PlacetoPay.Integrations.Library.CSharp.Contracts;
using PlacetoPay.Integrations.Library.CSharp.Entities;
using PlacetoPay.Integrations.Library.CSharp.Message;
using Tienda.Models;
using Tienda.Servicios.Interfaces;
using P2P = PlacetoPay.Integrations.Library.CSharp.PlacetoPay;

namespace Tienda.Controllers
{
    public class OrdersController : Controller
    {
        #region Atributos

        private readonly IServicioOrdenes _servicioOrdenes;
        private readonly IServicioPagos _servicioPagos;

        #endregion

        #region Constructor

        public OrdersController(IServicioOrdenes servicioOrdenes, IServicioPagos servicioPagos)
        {
            this._servicioOrdenes = servicioOrdenes;
            this._servicioPagos = servicioPagos;
        }

        #endregion

        #region Métodos

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            return View("Index", await _servicioOrdenes.ObtenerOrdenes());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id, string message)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Buscar información de la orden ObtenerOrden
            var order = await _servicioOrdenes.ObtenerOrden(id.Value);

            if (order == null)
            {
                return NotFound();
            }
            
            var pagosRechazados = false;

            // Verificar si tiene por lo menos un pago
            if (order.Payments.Count>0)
            {
                //Verificar si tiene un pago NO rechazado
                var pay = (from p in order.Payments
                          where p.Status != "REJECTED"
                           select p).FirstOrDefault();

                if (pay != null)
                {
                    if (pay.Status != "PAYED")
                    {
                        // Verificar el pago por RequesId
                        Gateway gateway = GetGateway();
                        RedirectInformation response = gateway.Query(pay.RequestId.ToString());

                        if (response.IsSuccessful())
                        {
                            switch (response.Status.status)
                            {
                                case "APPROVED":
                                    order.Status = "PAYED";
                                    order.UpdatedAt = Convert.ToDateTime(response.Status.Date);
                                    pay.Status = response.Status.status;
                                    pay.FechaUpdate = Convert.ToDateTime(response.Status.Date);
                                    break;

                                case "PENDING":
                                    pay.Status = response.Status.status;
                                    pay.FechaUpdate = Convert.ToDateTime(response.Status.Date);
                                    break;

                                case "REJECTED":
                                    pay.Status = response.Status.status;
                                    pay.FechaUpdate = Convert.ToDateTime(response.Status.Date);
                                    pay.Message = response.Status.Message;
                                    pagosRechazados = true;
                                    break;
                            }
                        }
                        else
                        {
                            message = response.Status.Message;
                        }

                        // Actualizar datos del pago y datos de la orden
                        if (await _servicioPagos.ActualizarPago(pay) == 0)
                        {
                            // NotFound Response Status 404.
                            return NotFound();
                        }

                        if (await _servicioOrdenes.ActualizarOrden(order) == 0)
                        {
                            // NotFound Response Status 404.
                            return NotFound();
                        }
                    }                    
                }
                else
                {
                    // Tiene pagos rechazados
                    pagosRechazados = true;
                }
            }

            ViewBag.PagosRechazados = pagosRechazados;
            ViewData["Success"] = message;

            return View("Details", order);
        }

        // GET: Orders/Payment/5
        public async Task<IActionResult> Payment(int? idOrden, string urlPago)
        {
            if (idOrden == null)
            {
                return NotFound();
            }
            
            var order = await _servicioOrdenes.ObtenerOrden(idOrden.Value); 

            if (order == null)
            {
                return NotFound();
            }

            ViewBag.UrlPago = urlPago == null ? "" : urlPago;

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment(int idOrden)
        {
            var order = await _servicioOrdenes.ObtenerOrden(idOrden);

            if (order == null)
            {
                return NotFound();
            }
            
            Gateway gateway = GetGateway();

            Person person = new Person(
                                        document: order.CustomerDocument,
                                        documentType: "CC",
                                        name: order.CustomerName,
                                        surname: order.CustomerName.Replace(" ", ""),
                                        email: order.CustomerEmail,
                                        mobile: order.CustomerMobile
                                        );
            
            Amount amount = new Amount(Convert.ToDouble(order.ValorOrder), "COP");
            PlacetoPay.Integrations.Library.CSharp.Entities.Payment payment = new PlacetoPay.Integrations.Library.CSharp.Entities.Payment($"TEST_{DateTime.Now:yyyyMMdd_hhmmss}_{order.Id}", $"Pago básico de prueba orden {order.Id} ", amount, false, person);
            RedirectRequest request = new RedirectRequest(payment,
                                                            "https://localhost:44336/Orders/Details/" + order.Id.ToString(),
                                                            "181.78.12.121",
                                                            "PlacetoPay Sandbox",
                                                            (order.CreatedAt.AddMinutes(60)).ToString("yyyy-MM-ddTHH:mm:sszzz"),
                                                            person,
                                                            person);

            RedirectResponse response = gateway.Request(request);

            if (response.IsSuccessful())
            {
                Models.Payment pago = new Models.Payment()
                {
                    OrderId = order.Id,
                    Fecha = Convert.ToDateTime(response.Status.Date),
                    RequestId = Convert.ToInt32(response.RequestId),
                    UrlPago = response.ProcessUrl,
                    Status = response.Status.status,
                    Reason = response.Status.Reason,
                    Message = response.Status.Message
                };

                if (await _servicioPagos.CrearPago(pago) == 0)
                {
                    // NotFound Response Status 404.
                    return NotFound();
                }


                return RedirectToAction("Payment", "Orders", new { idOrden = order.Id, urlPago = response.ProcessUrl });
            }
            else
            {                                
                return RedirectToAction("Details", "Orders", new { id = order.Id, message = response.Status.Message });
            }
        }
       
        // GET: Orders/Create
        public IActionResult Create(string codigo, string descripcion, string imagen, string valor)
        {
            ViewBag.CodigoProducto = codigo;
            ViewBag.NombreProducto = descripcion;
            ViewBag.ImagenProducto = imagen;
            ViewBag.ValorProducto = valor;

            return View("Create");
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order, string codigoProd, string descripcionProd, string valorProd)
        {
            if (ModelState.IsValid)
            {
                order.CreatedAt = DateTime.Now;
                order.UpdatedAt = DateTime.MinValue;
                order.ValorOrder = Convert.ToDouble(valorProd);
                order.Status = "CREATED";

                if (await _servicioOrdenes.CrearOrden(order) == 0)
                {
                    // NotFound Response Status 404.
                    return NotFound();
                }


                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderId = order.Id,
                    CodigoProducto = codigoProd,
                    NombreProducto = descripcionProd,
                    Cantidad = 1,
                    Valor = Convert.ToDouble(valorProd),
                    Total = Convert.ToDouble(valorProd)
                };

                if (await _servicioOrdenes.CrearOrdenDetalle(orderDetail) == 0)
                {
                    // NotFound Response Status 404.
                    return NotFound();
                }

                return RedirectToAction("Details", "Orders", new { id = order.Id});
            }
            return View("Create", order);
        }

        private Gateway GetGateway()
        {

            return new P2P(Environment.GetEnvironmentVariable("Login"),
                                      Environment.GetEnvironmentVariable("TranKey"),
                                      new Uri(Environment.GetEnvironmentVariable("UrlBase")),
                                      Gateway.TP_REST);
        }

        #endregion

    }
}
