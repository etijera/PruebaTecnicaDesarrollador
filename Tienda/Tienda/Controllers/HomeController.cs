﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tienda.Models;

namespace Tienda.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var productos= new List<Producto>() {   
                                                    new Producto { Codigo = "01", Descripcion = "Camisa Azul", Imagen = "/Images/Camisa1.jpg", Valor = 100 },
                                                    new Producto { Codigo = "02", Descripcion = "Camisa Roja", Imagen = "/Images/Camisa2.jpg", Valor = 150 },
                                                    new Producto { Codigo = "03", Descripcion = "Pantalón Rojo", Imagen = "/Images/Pantalon1.jpg", Valor = 200 },
                                                    new Producto { Codigo = "04", Descripcion = "Pantalón Verde", Imagen = "/Images/Pantalon2.jpg", Valor = 200 }
                                                };
            return View(productos);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Index(string codigo, string descripcion, string imagen, string valor)    
        {                
            return RedirectToAction("Create", "Orders",new { codigo = codigo, descripcion = descripcion, imagen = imagen, valor = valor });
        }
    }
}