using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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

//[assembly: ApiConventionType(typeof(DefaultApiConventions))]
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
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<ISellerRepository, SellersRepository>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IDispatchService, DispatchOrderService>();


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
                    // you can configure the 
                    //validate issuer, validate audience, valid audience, validissuer
                    //issueersigninngKey and the lIkes

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



            //   AddControllersWithViews
            services.AddControllers(
                  setupAction =>
                  {
                      setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                      setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized));
                      setupAction.Filters.Add(
                         new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
                      setupAction.Filters.Add(
                         new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
                      setupAction.ReturnHttpNotAcceptable = true;
                      var jsonOutputFormatter = setupAction.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>().FirstOrDefault();
                      if (jsonOutputFormatter == null)
                      {
                          // Remove test/Json as it isnt aproved media types
                          // Not working json at api level
                          if (jsonOutputFormatter.SupportedMediaTypes.Contains("text/json"))
                          {
                              jsonOutputFormatter.SupportedMediaTypes.Remove("text/json");
                          }
                      }
                  }
                //Where mY Experiment ends

                // Json excepetion stuff copied from stacKoVerflow  Prevents Json cyclic reference stuff
                ).AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling
                = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

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
            services.AddVersionedApiExplorer(setupAction =>
            {
                setupAction.GroupNameFormat = "'v'VV";
            });



            // Versioning Vibes

            services.AddApiVersioning(setupAction =>
            {
                setupAction.AssumeDefaultVersionWhenUnspecified = true;
                setupAction.DefaultApiVersion = new ApiVersion(1, 0);
                setupAction.ReportApiVersions = true;
            });
            var apiVersioningDescriptionProvider = 
                services.BuildServiceProvider().
                  GetService<IApiVersionDescriptionProvider>();


            // Swagger flow
            services.AddSwaggerGen(setupAction =>
            {
                foreach (var description in apiVersioningDescriptionProvider.ApiVersionDescriptions)
                {
                     setupAction.SwaggerDoc($"LibraryOpenApiSpecification{description.GroupName}",
               new Microsoft.OpenApi.Models.OpenApiInfo()
               {
                   Title = "StyleON Api",
                   Version = description.ApiVersion.ToString(),
                   Description = "Style on Api for E Commerce",


               });
                    setupAction.DocInclusionPredicate((documentName, apiDescription) =>
                    {
                        var actionApiVersionModel = apiDescription.ActionDescriptor
                         .GetApiVersionModel(ApiVersionMapping.Explicit | ApiVersionMapping.Implicit);
                        if (actionApiVersionModel == null)
                        {
                            return true;
                        }
                        if (actionApiVersionModel.DeclaredApiVersions.Any())
                        {
                            return actionApiVersionModel.DeclaredApiVersions.Any(
                                v => $"LibraryOpenApiSpecificationv{v.ToString()}" == documentName);
                        }
                        return actionApiVersionModel.ImplementedApiVersions.Any(v =>
                           $"LibraryOpenApiSpecification{v.ToString()}" == documentName);
                        // In the properties file we tick the Box to allow Xml Docu,netatio and
                        //the location to document and we erase to name of assenmbly


                       
                       
                    });
             
                  
                }
                
                var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
                setupAction.IncludeXmlComments(xmlCommentsFullPath);

                // Security Definotion flow
                setupAction.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                { 
                  Name = "Authorization",
                  Type = SecuritySchemeType.ApiKey,
                  Scheme = "Bearer",
                  BearerFormat = "JWT",
                  In = ParameterLocation.Header,
                  Description = "Enter 'Bearer' [space] and then your valid token in the text input below .\r\n\r\nExample: \"Bearer eyjhGciojdkrandomshitjvnvlvlvkkossps\"",

                });
                setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                  {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer" }
                        }, new List<string>() }

                });

            });


           

          

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IApiVersionDescriptionProvider apiVersioningDescriptionProvider)
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
                //setupAction.SwaggerEndpoint("/swagger/LibraryOpenApiSpecification/swagger.json",
                //    "StyleOn Api");
                ////  setupAction.RoutePrefix = "";
                foreach (var description in apiVersioningDescriptionProvider.ApiVersionDescriptions)
                {
                    setupAction.SwaggerEndpoint($"/swagger/" +
                     $"LibraryOpenApiSpecification{description.GroupName}/swagger.json",
                     description.GroupName.ToUpperInvariant());
                }
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


//

