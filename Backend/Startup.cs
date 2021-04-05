using Backend.Data;
using Backend.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                    .AddJsonOptions(opt =>
                    {
                        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    });
            services.AddDistributedMemoryCache();
            services.AddSession(opt =>
            {
                opt.Cookie.IsEssential = true;
            });
            services.AddDbContext<DatabaseContext>(ctx => 
                ctx.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")
                ));
            services.AddRepositories();
            services.AddHttpContextAccessor();
            services.AddGameSession();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend", Version = "v1" });
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opt =>
                    {
                        opt.Events = new CookieAuthenticationEvents
                        {
                            OnRedirectToLogin = ctx =>
                            {
                                ctx.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                                return Task.FromResult(0);
                            }
                        };
                    });
            services.AddCors(options =>
            {
                options.AddPolicy("CORS",
                builder =>
                {
                    builder.WithOrigins("https://localhost",
                                        "http://localhost",
                                        "http://localhost:8080",
                                        "*")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod()
                                        .WithExposedHeaders("x-quiz-session-id");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                              IWebHostEnvironment env, 
                              DatabaseContext ctx)
        {
            ctx.Database.EnsureCreated();

            try
            {
                ctx.Database.Migrate();
            }
            catch (Exception e)
            {
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CORS");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseSession();

            app.Use(async (context, next) =>
            {
                context.Response.OnStarting(state =>
                {
                    var httpContext = (HttpContext)state;

                    if (!httpContext.Request.Headers.TryGetValue("X-Quiz-Session-Id", out var sessionId)
                    || string.IsNullOrWhiteSpace(sessionId.ToString())
                    || sessionId.ToString() == "undefined")
                    {
                        var sessionIdNew = Guid.NewGuid().ToString();
                        if (!httpContext.Response.Headers.TryAdd("x-quiz-session-id", sessionIdNew))
                        {
                            httpContext.Response.Headers.Append("x-quiz-session-id", sessionIdNew);
                        }
                        try
                        {
                            httpContext.Session.SetString("SessionId", sessionIdNew);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else
                    {
                        httpContext.Response.Headers.TryAdd("x-quiz-session-id", sessionId);
                        try
                        {
                            context.Session.SetString("SessionId", sessionId);
                        }
                        catch (Exception)
                        {
                        }
                    }

                    httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                    return Task.CompletedTask;
                }, context);
                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("/index.html");
            });
        }
    }
}
