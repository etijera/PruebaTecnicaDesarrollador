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
using P2P = PlacetoPay.Integrations.Library.CSharp.PlacetoPay;

namespace Tienda.Controllers
{
    public class OrdersController : Controller
    {
        private readonly TiendaContext _context;

        public OrdersController(TiendaContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            return View(await _context.Orders.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id, string message)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Buscar información de la orden
            var order = await _context.Orders.Include("OrderDetails").Include("Payments")
                .FirstOrDefaultAsync(m => m.Id == id);

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

                        // Actualizar datos del pago y datos de la orden
                        _context.Update(pay);
                        _context.Update(order);
                        await _context.SaveChangesAsync();
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

            return View(order);
        }

        // GET: Orders/Payment/5
        public async Task<IActionResult> Payment(int? idOrden, string urlPago)
        {
            if (idOrden == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.Include("OrderDetails").Include("Payments")
                .FirstOrDefaultAsync(m => m.Id == idOrden);

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
            var order = await _context.Orders.Include("OrderDetails").Include("Payments")
               .FirstOrDefaultAsync(m => m.Id == idOrden);

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
            PlacetoPay.Integrations.Library.CSharp.Entities.Payment payment = new PlacetoPay.Integrations.Library.CSharp.Entities.Payment(order.Id.ToString(), "Pago básico de prueba " + order.Id.ToString(), amount, false, person);
            RedirectRequest request = new RedirectRequest(payment,
                                                            "https://localhost:44336/Orders/Details/" + order.Id.ToString(),
                                                            "181.78.12.121",
                                                            "PlacetoPay Sandbox",
                                                            (order.CreatedAt.AddMinutes(15)).ToString("yyyy-MM-ddTHH:mm:sszzz"));

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

                _context.Add(pago);
                await _context.SaveChangesAsync();

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

            return View();
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

                _context.Add(order);
                await _context.SaveChangesAsync();
                
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderId = order.Id,
                    CodigoProducto = codigoProd,
                    NombreProducto = descripcionProd,
                    Cantidad = 1,
                    Valor = Convert.ToDouble(valorProd),
                    Total = Convert.ToDouble(valorProd)
                };

                _context.Add(orderDetail);
                await _context.SaveChangesAsync();
                
                return RedirectToAction("Details", "Orders", new { id = order.Id});
            }
            return View(order);
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

        private Gateway GetGateway()
        {

            return new P2P(Environment.GetEnvironmentVariable("Login"),
                                      Environment.GetEnvironmentVariable("TranKey"),
                                      new Uri(Environment.GetEnvironmentVariable("UrlBase")),
                                      Gateway.TP_REST);
        }        

    }
}
