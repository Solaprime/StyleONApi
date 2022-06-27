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
            modelBuilder.Entity<Product>().HasData(
                 new Product()
                 {
                      ProductId = Guid.NewGuid(),
                      Name = "Dior Bag",

                 },
                 new Product()
                 {
                     ProductId= Guid.Parse("da2fd609-d754-4feb-8acd-c4f9ff13ba96"),
                     Name = "Shoe"

                 }
                 );
        }
    }
}
