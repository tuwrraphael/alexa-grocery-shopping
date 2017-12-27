using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillaSkill.Billa;
using BillaSkill.Impl;
using BillaSkill.Models;
using BillaSkill.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BillaSkill
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            new DbAccess(new DbConnectionOptions()
            {
                DB_KEY = Configuration["DB_KEY"],
                DB_URI = Configuration["DB_URI"]
            }).InitializeDb().Wait();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddTransient<ILieferant, BillaService>();
            services.AddTransient<IWarenFormatter, WarenFormatter>();
            services.Configure<DbConnectionOptions>(Configuration);
            services.Configure<LieferantCredentials>(Configuration);
            services.AddTransient<IDbAccess, DbAccess>();
            services.AddTransient<IWarenkorbRepository, WarenkorbRepository>();
            services.AddTransient<ISucheRepository, SucheRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
