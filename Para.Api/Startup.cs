using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Para.Business.Mappings;
using Para.Business.Validatiors;
using Para.Data.Context;
using Para.Data.UnitOfWork;

namespace Para.Api;

public class Startup
{
    public IConfiguration Configuration;
    
    public Startup(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }

    [Obsolete]
    public void ConfigureServices(IServiceCollection services)
    {

        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.WriteIndented = true;
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
        });
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Para.Api", Version = "v1" });
        });

        var connectionStringSql = Configuration.GetConnectionString("MsSqlConnection");
        services.AddDbContext<ParaSqlDbContext>(options => options.UseSqlServer(connectionStringSql));

        var connectionStringPostgre = Configuration.GetConnectionString("PostgresSqlConnection");
        services.AddDbContext<ParaPostgreDbContext>(options => options.UseNpgsql(connectionStringPostgre));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Validasyon tanýmlamalarý konfigrasyonu.
        services.AddFluentValidation(opt =>
        {
            opt.RegisterValidatorsFromAssemblyContaining<CustomerValidator>();
            opt.RegisterValidatorsFromAssemblyContaining<CustomerDetailValidator>();
            opt.RegisterValidatorsFromAssemblyContaining<CustomerAddressValidator>();
            opt.RegisterValidatorsFromAssemblyContaining<CustomerPhoneValidator>();
        });
        //Dto entity dönüþümü için automapper konfigrasyonu.
        services.AddAutoMapper(opt =>
        {
            opt.AddProfile<CustomerProfile>();
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Para.Api v1"));
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}