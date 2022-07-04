using Microsoft.EntityFrameworkCore;
using StyleONApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Context
{
    public class StyleONContext  : DbContext
    {
        public StyleONContext(DbContextOptions options) : base(options)
        {
                     
        }
        public DbSet<Product> Products { get; set; }
     
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
<<<<<<< HEAD
         
=======

            
>>>>>>> 0e8e96d9364b6b5479621b2c91a2f3a1cbf646c6
        }
    }
}
