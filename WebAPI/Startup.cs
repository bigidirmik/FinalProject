using Business.Abstract;
using Business.Concrete;
using Core.DependencyResolvers;
using Core.Extensions;
using Core.Utilities.IoC;
using Core.Utilities.Security.Encryption;
using Core.Utilities.Security.JWT;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI
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
            // bu yap�y� farkl� mimariye ta��yaca��z
            //Autofac, Ninject, Castlewindsor, StructureMap, LightInject, DryInject --> IoC yap�s� yokken de bunlar ile bu yap� kurulurdu.
            //AOP - Autofac en iyi AOP imkan�n� sunar.
            // Cross Cutting Concerns - Uygulamay� dikine kesen ilgi alanlar�
            //(�rn: loglama,cache,transaction:performans,authorization (aray�z do�rulama,data do�rulama, business...))

            services.AddControllers();
            //services.AddSingleton<IProductService,ProductManager>();
            //services.AddSingleton<IProductDal,EfProductDal>();

            services.AddCors();

            //JWT

            var tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = tokenOptions.Issuer,
                        ValidAudience = tokenOptions.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
                    };
                });

            //Core'da yapt���m�z extension sayesinde istedi�imiz module'� ekleyebiliriz. AspectModule, SecurityModule gibi...
            //Array yerine params ve koleksiyon olarak da ekleyebiliriz..
            services.AddDependencyResolvers(new ICoreModule[] { 
                new CoreModule()
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ConfigureCustomExceptionMiddleware();

            app.UseCors(builder=>builder.WithOrigins("http://localhost:4200").AllowAnyHeader()); // bu adresten gelen her iste�e izin ver demek. frontend

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
