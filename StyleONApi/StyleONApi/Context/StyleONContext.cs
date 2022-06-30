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
        public DbSet<Seller> Sellers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            
        }
    }
}
