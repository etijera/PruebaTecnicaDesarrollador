using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Tienda.Models;

namespace Tienda
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            //// COMPRUEBA QUE LA BD ESTÁ CREADA Y SI NO LA CREA.
            //Database.EnsureCreated();
        }

        #region DbSet

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }

        #endregion
    }
}
