using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Runtime;
using PlaniranjePutovanja.Common.Interfaces.Auth;
using PlaniranjePutovanja.Common.Interfaces.Expense;
using PlaniranjePutovanja.Common.Interfaces.Travel;
using PlaniranjePutovanja.Common.Interfaces.Util;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace APIGateway
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class APIGateway : StatelessService
    {
        public APIGateway(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        var builder = WebApplication.CreateBuilder();

                        builder.Services.AddSingleton<StatelessServiceContext>(serviceContext);
                        builder.Services.AddSingleton<IAuthService>(provider =>
                            ServiceProxy.Create<IAuthService>(new Uri("fabric:/PlaniranjePutovanja/PlaniranjePutovanja.AuthService")));

                        builder.Services.AddSingleton<ITravelService>(provider =>
                            ServiceProxy.Create<ITravelService>(new Uri("fabric:/PlaniranjePutovanja/PlaniranjePutovanja.TravelService")));

                        builder.Services.AddSingleton<IExpenseService>(provider =>
                            ServiceProxy.Create<IExpenseService>(new Uri("fabric:/PlaniranjePutovanja/PlaniranjePutovanja.ExpenseService"), new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(0)));

                        builder.Services.AddSingleton<IUtilService>(provider =>
                            ServiceProxy.Create<IUtilService>(new Uri("fabric:/PlaniranjePutovanja/PlaniranjePutovanja.UtilService")));

                         builder.Services.AddHttpContextAccessor();

                        // Ucitavamo JWT konfiguraciju iz Service Fabric konfiguracije
                        var configPackage = serviceContext.CodePackageActivationContext.GetConfigurationPackageObject("Config");
                        var jwtSecretKey = configPackage.Settings.Sections["SecurityConfiguration"].Parameters["JwtSecretKey"].Value;
                        var jwtIssuer = configPackage.Settings.Sections["SecurityConfiguration"].Parameters["JwtIssuer"].Value;
                        var jwtAudience = configPackage.Settings.Sections["SecurityConfiguration"].Parameters["JwtAudience"].Value;

                        builder.Services.AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                            options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                        })
                        .AddJwtBearer(options =>
                        {
                            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidIssuer = jwtIssuer,
                                ValidateAudience = true,
                                ValidAudience = jwtAudience,
                                ValidateLifetime = true,
                                ClockSkew = TimeSpan.Zero,
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSecretKey))
                            };
                        });

                        builder.Services.AddAuthorization(options =>
                        {
                            options.AddPolicy("CanViewTravel", policy =>
                                policy.Requirements.Add(new PlaniranjePutovanja.APIGateway.Authorization.TravelAccessRequirement(PlaniranjePutovanja.APIGateway.Authorization.AccessRequirement.View)));

                            options.AddPolicy("CanEditTravel", policy =>
                                policy.Requirements.Add(new PlaniranjePutovanja.APIGateway.Authorization.TravelAccessRequirement(PlaniranjePutovanja.APIGateway.Authorization.AccessRequirement.Edit)));
                        });

                        builder.Services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, PlaniranjePutovanja.APIGateway.Authorization.TravelAccessHandler>();

                        builder.WebHost
                                    .UseKestrel()
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url);
                        builder.Services.AddControllers();
                        builder.Services.AddEndpointsApiExplorer();
                        //builder.Services.AddSwaggerGen();
                        builder.Services.AddSwaggerGen(c =>
                        {
                            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Travel Planner API", Version = "v1" });

                            // Konfigurisemo Swagger da koristi JWT Bearer token
                            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                            {
                                Description = "JWT Authorization header using the Bearer scheme. Primer: \"Bearer {token}\"",
                                Name = "Authorization",
                                In = ParameterLocation.Header,
                                Type = SecuritySchemeType.Http,
                                Scheme = "bearer",
                                BearerFormat = "JWT"
                            });

                            c.AddSecurityRequirement(new OpenApiSecurityRequirement
                            {
                                {
                                    new OpenApiSecurityScheme
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.SecurityScheme,
                                            Id = "Bearer"
                                        }
                                    },
                                    new string[] {}
                                }
                            });
                        });

                        builder.Services.AddCors(options =>
                        {
                            options.AddPolicy("FrontendCorsPolicy", policy =>
                            {
                                policy
                                    .WithOrigins("http://localhost:5173") 
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                            });
                        });
                        var app = builder.Build();
                        //if (app.Environment.IsDevelopment())
                        //{
                        //app.UseSwagger();
                        //app.UseSwaggerUI();
                        //}
                        app.UseSwagger();
                        app.UseSwaggerUI();
                        app.UseRouting();
                        app.UseCors("FrontendCorsPolicy");
                        app.UseAuthentication();
                        app.UseAuthorization();
                        app.MapControllers();
                        
                        return app;

                    }))
            };
        }
    }
}
