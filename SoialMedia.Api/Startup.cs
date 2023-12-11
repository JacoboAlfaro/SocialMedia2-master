using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SocialMedia.Infrastructure.Extensions;
using SocialMedia.Infrastructure.Filters;
using System;
using System.Reflection;
using System.Text;

namespace SoialMedia.Api
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
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                //options.SuppressModelStateInvalidFilter = true;
            });
            services.AddOptions(Configuration);
            services.AddDbContexts(Configuration);
            services.AddServices();
            services.AddSwagger($"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Authentication:Issuer"],
                    ValidAudience = Configuration["Authentication:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Authentication:SecretKey"]))
                };
            });

            services.AddMvc(options =>
            {
                options.Filters.Add<ValidationFilter>();
            }).AddFluentValidation(options =>
            {
                options.RegisterValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost", builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });

                //    services.AddCors(options =>
                //    {
                //        options.AddPolicy("AllowLocalhost", builder =>
                //        {
                //            builder.WithOrigins("http://localhost:4200")  // Reemplaza con tu origen Angular
                //                .AllowAnyMethod()
                //                .AllowAnyHeader()
                //                .AllowCredentials();


                //        });

                //        //services.AddCors(options =>
                //        //{
                //        //    options.AddPolicy("CorsPolicy",
                //        //        builder => builder.AllowAnyOrigin()
                //        //        .AllowAnyMethod()
                //        //        .AllowAnyHeader()
                //        //        .AllowCredentials());
                //        //});
                //    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("AllowLocalhost");
            //app.UseCors("AllowLocalhost");

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                //Local
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Social Media API V1");
                options.RoutePrefix = string.Empty;
                //IIS Server
                //options.SwaggerEndpoint("../swagger/v1/swagger.json", "Social Media API V1");


            });
            //app.UseCors(options => options.AllowAnyOrigin());

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
