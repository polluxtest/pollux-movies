using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Pollux.Application.Mappers;
using Pollux.Persistence;

namespace Pollux.Movies
{
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration<see cref="IConfiguration"/>.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the Configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// The ConfigureServices.
        /// </summary>
        /// <param name="services">The services<see cref="IServiceCollection"/>.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = this.Configuration.GetSection("AppSettings")["DbConnectionStrings:PolluxSQLConnectionString"];
            services.AddDbContext<PolluxMoviesDbContext>(options => options.UseSqlServer(connectionString));
            services.AddCors();
            services.AddMvc();
            services.AddAuthorization();
            services.AddControllers();
            services.AddSwaggerGen();
            this.SetUpSwagger(services);
            services.AddDIRepositories();
            services.AddDIServices();
            services.AddAutoMapper(AssemblyApplication.Assembly);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            this.AddSwagger(app);
            app.UseCors(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(
            endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute("default", "{controller}/{action}/{id}");
            });
        }

        /// <summary>
        /// Adds the swagger.
        /// </summary>
        /// <param name="app">The application.</param>
        private void AddSwagger(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });
        }

        /// <summary>
        /// Sets up swagger.
        /// </summary>
        /// <param name="services">The services.</param>
        private void SetUpSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description = @"Bearer {access token}",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                    });

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement()
                        {
                            {
                                new OpenApiSecurityScheme
                                    {
                                        Reference = new OpenApiReference{ Type = ReferenceType.SecurityScheme, Id = "Bearer"},
                                        Scheme = "oauth2",
                                        Name = "Bearer",
                                        In = ParameterLocation.Header,
                                    },
                                new List<string>()
                            },
                        });
            });
        }
    }
}
