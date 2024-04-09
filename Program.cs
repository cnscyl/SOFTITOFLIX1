using SOFTITOFLIX.Data;
using SOFTITOFLIX.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SOFTITOFLIX.DTO.Converters;
using System.Text.Json.Serialization;

namespace SOFTITOFLIX;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<SOFTITOFLIXContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDatabase")));

        builder.Services.AddIdentity<SOFTITOFLIXUser,SOFTITOFLIXRole>().AddEntityFrameworkStores<SOFTITOFLIXContext>().AddDefaultTokenProviders();

        builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);




        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //If you are using dependency injection on controller you have to add this.
        //builder.Services.AddScoped<MediaConverter>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();
        {
            SOFTITOFLIXContext? context = app.Services.CreateScope().ServiceProvider.GetService<SOFTITOFLIXContext>();
            SignInManager<SOFTITOFLIXUser>? signInManager = app.Services.CreateScope().ServiceProvider.GetService < SignInManager<SOFTITOFLIXUser>>();
            RoleManager<SOFTITOFLIXRole>? roleManager = app.Services.CreateScope().ServiceProvider.GetService<RoleManager<SOFTITOFLIXRole>>();

            DataInitialization dataInitialization = new DataInitialization(context!, signInManager!, roleManager!);
        }



        app.Run();
    }
}

