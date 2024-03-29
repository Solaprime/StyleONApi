using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StyleONApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Context
{
    public class StyleONContext  : IdentityDbContext<ApplicationUser>
    {
        public StyleONContext(DbContextOptions options) : base(options)
        {
                     
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<Dispatch> Dispatchs { get; set; }
        public DbSet<Order> Orders { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

        
    }
}


// When u inherifted from IdentityDbContext,  i encounter a  
//"The entity type 'IdentityUserLogin<string>' requires a primary key to be defined [duplicate]" error i solved it by adding 


// base.OnModelCreating(modelBuilder);