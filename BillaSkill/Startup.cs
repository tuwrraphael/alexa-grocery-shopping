using System;
using System.IO;
using BillaSkill.Billa;
using BillaSkill.Impl;
using BillaSkill.Impl.CosmosDb;
using BillaSkill.Impl.FileStore;
using BillaSkill.Models;
using BillaSkill.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace BillaSkill
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
            if (string.IsNullOrWhiteSpace(hostingEnvironment.WebRootPath))
            {
                hostingEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }
            if (!string.IsNullOrEmpty(Configuration["DB_KEY"]) && !string.IsNullOrEmpty(Configuration["DB_URI"]))
            {
                UseCosmosDb = true;
            }
            else
            {
                UseCosmosDb = false;
            }
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        public bool UseCosmosDb { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddTransient<ILieferant, BillaService>();
            services.AddTransient<IWarenFormatter, WarenFormatter>();
            services.Configure<AzureAesKeyOptions>(Configuration);
            services.AddTransient<IAESKeyProvider, AzureAESKeyProvider>();
            services.AddTransient<ICredentialEncryption, AesCredentialEncryption>();
            if (UseCosmosDb)
            {
                services.Configure<DbConnectionOptions>(Configuration);
                new DbAccess(new DbConnectionOptions()
                {
                    DB_KEY = Configuration["DB_KEY"],
                    DB_URI = Configuration["DB_URI"]
                }).InitializeDb().Wait();
                services.AddTransient<IDbAccess, DbAccess>();
                services.AddTransient<IWarenkorbRepository, WarenkorbRepository>();
                services.AddTransient<ISucheRepository, SucheRepository>();
            }
            else
            {
                var path = Path.Combine(HostingEnvironment.WebRootPath, "App_Data");
                new FileStoreAccess(null, path).InitializeAsync().Wait();
                var provider = new PhysicalFileProvider(path);
                var access = new FileStoreAccess(provider, path);
                services.AddSingleton<IFileProvider>(provider);
                services.AddSingleton<IFileAccess>(access);
                services.AddTransient<ISucheRepository, FileStoreSucheRepository>();
                services.AddTransient<IWarenkorbRepository, FileStoreWarenkorbRepository>();
                services.AddTransient<IUserRepository, FileStoreUserRepository>();
            }
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
