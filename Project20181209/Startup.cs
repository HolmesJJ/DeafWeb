using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectData;
using ProjectData.Models;
using ProjectServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Project20181209.Tokens;

namespace Project20181209
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
            services.AddMvc();
            services.AddSingleton(Configuration);
            services.AddScoped<IProject, ProjectService>();

            services.AddDbContext<ProjectContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("ProjectConnection")));

            // cookie Token
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = JWTTokenOptions.Issuer, // 设置签发者必须是 TestIssuer
                        ValidAudience = JWTTokenOptions.Audience, // 设置接收者必须是 TestAudience
                        IssuerSigningKey = JWTTokenOptions.Key(),
                        RequireSignedTokens = true,
                        RequireExpirationTime = true,
                        SaveSigninToken = false,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };

                    // 捕捉访问令牌是否已经过期
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("act", "expired");
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler("/api/Error");
            app.UseStatusCodePagesWithReExecute("/api/Error/HttpError/{0}");

            app.UseStaticFiles();

            // UseAuthentication must above UseMvc
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Project}/{action=Login}/{id?}");
            });
        }
    }
}
