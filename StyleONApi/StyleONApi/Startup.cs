using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using StyleONApi.AuthServices;
using StyleONApi.Context;
using StyleONApi.Entities;
using StyleONApi.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyleONApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(setupAction =>
                {
                    setupAction.SerializerSettings.ContractResolver
                      = new CamelCasePropertyNamesContractResolver();
                });
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddDbContext<StyleONContext>(options => options.UseSqlServer(Configuration.GetConnectionString("StyleONDb")).EnableSensitiveDataLogging());

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Our Authentication flow
            // the first part tells asp.net authentication flow to use the JWrBearerDefaults
            // found in the JWtBearer flow
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(jwt =>
                {
                    var key = Encoding.ASCII.GetBytes(Configuration.GetSection("JwtConfig:Secret").Value);
                    jwt.SaveToken = true;
                    jwt.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false, // this is only for development mode 
                        ValidateAudience = false, // this is only for development mode
                        RequireExpirationTime = false,  // this is only for development mode in real life token expired and theny need to be refreshed
                        ValidateLifetime = true


                    };
                }
             );

            // Adding some Identity Stuff

            services.AddIdentity<ApplicationUser, IdentityRole>(
                options =>
                {
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false; //  CHANGE TO TRUE
                    options.Password.RequireDigit = false;    // chNGE TO TRUE
                    options.Password.RequireLowercase = false;   // CHANGE TO TRUE
                    options.Password.RequiredLength = 5;
                    options.User.RequireUniqueEmail = true;
                    //options.SignIn.RequireConfirmedAccount = false;
                }).AddEntityFrameworkStores<StyleONContext>().AddDefaultTokenProviders();


            services.AddScoped<IUserService, UserService>();

            // Json excepetion stuff copied from stacKoVerflow

                //services.AddControllersWithViews().AddNewtonsoftJson(options =>
                //options.SerializerSettings.ReferenceLoopHandling
                //= Newtonsoft.Json.ReferenceLoopHandling.Ignore);

                //services.AddControllers().AddJsonOptions(options =>
                //{
                //    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                //    options.JsonSerializerOptions.WriteIndented = true;
                //});


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
