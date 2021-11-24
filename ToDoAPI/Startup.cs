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
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoAPI.Database;
using ToDoAPI.Models;
using ToDoAPI.Repositories;
using ToDoAPI.Repositories.Interfaces;

namespace ToDoAPI
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
            services.Configure<ApiBehaviorOptions>(cfg => {
                cfg.SuppressModelStateInvalidFilter = true;
            });

            services.AddMvc().AddNewtonsoftJson(cfg =>
            {
                cfg.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.AddDbContext<ToDoContext>(cfg => {
                cfg.UseSqlite("Data Source=Database\\ToDo.db");
            });

            // Dependencies Repos
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserTaskRepository, UserTaskRepository>();

            //Identity Config
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ToDoContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(cfg => {
                cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            services.ConfigureApplicationCookie(cfg => {
                cfg.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

            services.AddControllers();

/*            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDoAPI", Version = "v1" });
            });*/
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                /*app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDoAPI v1"));*/
            }

            app.UseHttpsRedirection();
            app.UseStatusCodePages();
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
