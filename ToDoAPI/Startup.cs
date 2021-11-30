using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoAPI.Database;
using ToDoAPI.V1.Helpers.Swagger;
using ToDoAPI.V1.Models;
using ToDoAPI.V1.Repositories;
using ToDoAPI.V1.Repositories.Interfaces;

namespace ToDoAPI
{
    public class Startup
    {
        // DI Inject
        #region DI Inject
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        #endregion
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Config Method
            #region Configure Method
            services.Configure<ApiBehaviorOptions>(cfg => {
                cfg.SuppressModelStateInvalidFilter = true;
            });
            #endregion

            // AddMvc - API - Config
            #region Add Mvc - Config
            services.AddMvc();

            #endregion  

            // Api Versioning
            #region API Versioning - Config
            services.AddApiVersioning(cfg => {
                cfg.ReportApiVersions = true;
                cfg.AssumeDefaultVersionWhenUnspecified = true;
                cfg.DefaultApiVersion =  new ApiVersion(1, 0);
            });
            #endregion

            // Database - Config
            #region DB - Config
            services.AddDbContext<ToDoContext>(cfg => {
                cfg.UseSqlite("Data Source=Database\\ToDo.db");
            });
            #endregion

            //  Dependencies Repos
            #region Dependency Inject - Config
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserTaskRepository, UserTaskRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            #endregion

            //  Identity - Config
            #region Identity - Config
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ToDoContext>()
                .AddDefaultTokenProviders();
            #endregion

            // Authentication - Config
            #region Authentication & JWT - Config
            services.AddAuthentication(cfg => {
                cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(cfg => {
                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("key-api-jwt-to-do-application"))
                };
            });
            #endregion

            //Authorization - Config
            #region Authorization - Config
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                                            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                                            .RequireAuthenticatedUser()
                                            .Build()
                );
            });

            services.ConfigureApplicationCookie(cfg => {
                cfg.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });
            #endregion

            //Add Controllers - Config
            #region Add Controllers - Config
            services.AddControllers(cfg =>
            {
                cfg.RespectBrowserAcceptHeader = false;
                cfg.ReturnHttpNotAcceptable = true;
                cfg.InputFormatters.Add(new XmlSerializerInputFormatter(cfg));
                cfg.OutputFormatters.Add(new XmlSerializerOutputFormatter());
            }).AddNewtonsoftJson();
            #endregion

            //Swagger - Config
            #region  Swagger Doc & Api Swagger Versioning - Config  
            services.AddSwaggerGen(cfg =>
            {
                cfg.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() { 
                    In = (ParameterLocation)1,
                    Type = 0,
                    Description = "Adicione o JSON WEB TOKEN para autenticar.",
                    Name = "Authorization"
                });

                cfg.AddSecurityRequirement(new OpenApiSecurityRequirement{
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference{
                            Id = "Bearer", //The name of the previously defined security scheme.
                            Type = ReferenceType.SecurityScheme
                        }
                    },new List<string>()
                }
            });

                cfg.ResolveConflictingActions(apiDescription => apiDescription.First());
                cfg.DocInclusionPredicate((_, api) => !string.IsNullOrWhiteSpace(api.GroupName));
                cfg.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDoAPI - v1.0", Version = "v1" });

                var projectPath = PlatformServices.Default.Application.ApplicationBasePath;
                var projectName = $"{PlatformServices.Default.Application.ApplicationName}.xml";
                var pathXMLFileDoc = Path.Combine(projectPath, projectName);

                cfg.IncludeXmlComments(pathXMLFileDoc);
                
                cfg.DocInclusionPredicate((docName, apiDesc) => {
                    var actionApiVersionModel = apiDesc.ActionDescriptor?.GetApiVersion();
                    if (actionApiVersionModel == null)
                    {
                        return true;
                    }
                    if (actionApiVersionModel.DeclaredApiVersions.Any())
                    {
                        return actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName);
                    }
                    return actionApiVersionModel.ImplementedApiVersions.Any(v => $"v{v.ToString()}" == docName);
                });

                cfg.OperationFilter<ApiVersionOperationFilter>();
            });
            
            
            services.AddApiVersioning(cfg => {
                cfg.ReportApiVersions = true;
                cfg.AssumeDefaultVersionWhenUnspecified = true;
                cfg.DefaultApiVersion = new ApiVersion(1, 0);
            });
            #endregion

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Env Config
            #region Development Env - Config
            if (env.IsDevelopment())
            {
                app.UseRouting();
                app.UseStatusCodePages();


                app.UseEndpoints(endpoints => endpoints.MapControllers());
            }
            #endregion

            // Use APP - Config
            #region Use APP - Config
            app.UseHttpsRedirection();
            app.UseStatusCodePages();          
            app.UseRouting();
            app.UseAuthorization();
            app.UseAuthentication();
            #endregion

            // Endpoints - Config
            #region Endpoints APP - Config
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            #endregion

            // Swagger APP config
            #region Swagger App - Config  
            app.UseSwagger(); // /swagger/v1/swagger.json
            app.UseSwaggerUI(cfg => {
                cfg.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDoAPI v1");
            });
            #endregion
        }

    }
}
