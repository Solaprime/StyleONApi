using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
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

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISellerRepository, SellersRepository>();


            services.AddDbContext<StyleONContext>(options => options.UseSqlServer(Configuration.GetConnectionString("StyleONDb")).EnableSensitiveDataLogging());

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddControllers()
                .AddNewtonsoftJson(setupAction =>
                {
                    setupAction.SerializerSettings.ContractResolver
                      = new CamelCasePropertyNamesContractResolver();
                });
         

            // Our Authentication flow
            // the first part tells asp.net authentication flow to use the JWrBearerDefaults
            // found in the JWtBearer flow



            // to retrieve the Key from our AppSettings
            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("JwtConfig:Secret").Value);
            // This is will be a token validation parameter to work with the token and refreshtoken flow
            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false, // this is only for development mode 
                ValidateAudience = false, // this is only for development mode
                // check the requireexpirationtime property properlu
                RequireExpirationTime = false,  // this is only for development mode in real life token expired and theny need to be refreshed
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero

            };
            services.AddSingleton(tokenValidationParams);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(jwt =>
                {

                    jwt.SaveToken = true;
                    jwt.TokenValidationParameters = tokenValidationParams;
                  
                });

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



            // Json excepetion stuff copied from stacKoVerflow
            //   AddControllersWithViews
            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling
                = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

         

            // Swagger flow
            services.AddSwaggerGen(setupAction => 
            {
                setupAction.SwaggerDoc("LibraryOpenApiSpecification",
                    new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = "StyleON Api",
                        Version = "1",
                         Description = "Style on Api for E Commerce",


                    });
                // In the properties file we tick the Box to allow Xml Docu,netatio and
                //the location to document and we erase to name of assenmbly
                var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
                setupAction.IncludeXmlComments(xmlCommentsFullPath);
            });
            services.Configure<ApiBehaviorOptions>(options =>
            {


                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var actionExecutingContext =
                     actionContext as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

                // If there are modelstate erros and all Keys were correctly
                // Found/Parsed we're dealing with validation errors
                if (actionContext.ModelState.ErrorCount > 0
                      && actionExecutingContext?.ActionArguments.Count == actionContext.ActionDescriptor.Parameters.Count)
                    {
                        return new UnprocessableEntityObjectResult(actionContext.ModelState);
                    }

                  // If one of the Keys wasnt correctly found/ couldnt be parsed
                  // we are dealing with null/unparsable input
                    return new BadRequestObjectResult(actionContext.ModelState);
                };
            });

            services.AddMvc(setupAction =>
            {
                //setupAction.Filters.Add(
                //    new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                //setupAction.Filters.Add(
                //   new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
                //setupAction.Filters.Add(
                //   new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));

                setupAction.ReturnHttpNotAcceptable = true;
                var jsonOutPutForMatter = setupAction.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>().FirstOrDefault();
                if (jsonOutPutForMatter == null)
                {
                    // Remove text/Json as it isnt the approved media types
                    //Not working with Json at Api level
                    if (jsonOutPutForMatter.SupportedMediaTypes.Contains("text/json"))
                    {
                        jsonOutPutForMatter.SupportedMediaTypes.Remove("text/json");
                    }
                   
                }
            }).SetCompatibilityVersion(CompatibilityVersion.Latest);
               
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // Best practicr to place this after HttpsRedirection
            app.UseSwagger();
            // Swagger Ui flow 
            app.UseSwaggerUI(setupAction =>
            {
                setupAction.SwaggerEndpoint("/swagger/LibraryOpenApiSpecification/swagger.json",
                    "StyleOn Api");
              //  setupAction.RoutePrefix = "";
            });
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


// Base route for swagger opneApi speciication
//  https://localhost:44385/swagger/LibraryOpenApiSpecification/swagger.json
// https://localhost:44385/swagger/index.html


//klalalalallala

