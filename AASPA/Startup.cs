using AASPA.Domain.Interface;
using AASPA.Domain.Service;
using AASPA.Domain.Util;
using AASPA.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AASPA.App.Middleware;
using AASPA.Controllers;

namespace AASPA
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

            services.AddControllersWithViews();

            services.AddDbContext<MysqlContexto>(options => options
                .UseMySql(Configuration["MySQLConnection:MySQLConnectionString"], new MySqlServerVersion(new Version(8, 0, 5)))
                .EnableSensitiveDataLogging());

            services.AddCors(x=> x.AddDefaultPolicy(c=> c.AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader()));
            services.AddHttpClient<ClienteService>(client =>
            {
                client.BaseAddress = new Uri("https://integraall.com/api/");
            });
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(UnauthorizedMiddleware));
            });

            var key = Encoding.ASCII.GetBytes(Autenticacao.Settings.Secret);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddScoped<ICliente,ClienteService>();
            services.AddScoped<IStatus,StatusService>();
            services.AddScoped<IBeneficio,BeneficioService>();
            services.AddScoped<IPagamento, PagamentoService>();
            services.AddScoped<IOrigem, OrigemService>();
            services.AddScoped<IMotivoContato, MotivoContatoService>();
            services.AddScoped<IHistoricoContatoOcorrencia, HistoricoContatoOcorrenciaService>();
            services.AddScoped<IUsuario, UsuarioService>();
            services.AddScoped<IRemessa, RemessaService>();
            services.AddScoped<IRelatorios, RelatorioService>();
            services.AddScoped<ICaptador, CaptadorService>();
            services.AddScoped<IReembolso, ReembolsoService>();
            services.AddScoped<ILog, LogService>();
            services.AddScoped<IRepasse, RepasseService>();
            services.AddScoped<IIntegracaoKompleto, IntegracaoKompletoService>();
            services.AddScoped<ILogCancelamento, LogCancelamentoService>();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
