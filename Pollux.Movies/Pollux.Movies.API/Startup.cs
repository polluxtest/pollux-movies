using Movies.Application.Mappers;
using Movies.Persistence;

namespace Pollux.Movies
{
    using System.Collections.Generic;
    using AzureUploaderTransformerVideos;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;


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
            var connectionString = this.Configuration.GetSection("AppSettings")["DbConnectionStrings:PolluxMoviesSQLConnectionString"];
            services.AddDbContext<PolluxMoviesDbContext>(options => options.UseSqlServer(connectionString));
            this.AddAzureMediaServices(services);
            this.AddCors(services);
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
            app.UseCors("CookiePolicy");
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
        /// Adds the azure media service settings.
        /// </summary>
        /// <param name="services">The services.</param>
        private void AddAzureMediaServices(IServiceCollection services)
        {
            var azureMediaServiceSettings = new AzureMediaServiceConfig();
            this.Configuration.Bind("AzureMediaServiceSettings", azureMediaServiceSettings);

            services.AddSingleton(azureMediaServiceSettings);
            services.AddTransient<AzureMediaService>();
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
        /// Adds the cors.
        /// </summary>
        /// <param name="services">The services.</param>
        private void AddCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(
                    "CookiePolicy",
                    builder =>
                        builder.WithOrigins("https://localhost:3000")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials());
            });
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
