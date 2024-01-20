using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Globalization;
using trackingapi.Controllers;
using trackingapi.Data;
using trackingapi.Resources.controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddLocalization(options => options.ResourcesPath = "E:\\Git\\.NET\\trackingapi\\trackingapi\\Resources\\controllers\\AppResource.cs");
List<CultureInfo> supportedCultures = new()
{
    new CultureInfo("en"),
    new CultureInfo("vi"),
    new CultureInfo("")
};
builder.Services.AddControllers().AddViewLocalization();
//builder.Services.Configure<RequestLocalizationOptions>(options =>
//{
//    options.DefaultRequestCulture = new RequestCulture(culture: "en");
//    options.SupportedCultures = supportedCultures;
//    options.SupportedUICultures = supportedCultures; // important bit
//    options.RequestCultureProviders = new[] { new RouteDataRequestCultureProvider { IndexOfCulture = 3 } };
//});


builder.Services
                .Configure<RequestLocalizationOptions>(options =>
                {
                    var cultures = new[]
                    {
                        new CultureInfo("en"),
                        new CultureInfo("vi")
                    };
                    options.DefaultRequestCulture = new RequestCulture("en");
                    options.SupportedCultures = cultures;
                    options.SupportedUICultures = cultures;
                });

builder.Services.AddTransient<IStringLocalizer<AppResource>, StringLocalizer<AppResource>>();
//builder.Services.AddDbContext<IssueDbContext>(
//    o => o.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
builder.Services.AddDbContext<IssueDbContext>(
    o => o.UseSqlite("Data Source=Db/issue.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseRequestLocalization(new RequestLocalizationOptions
//{
//    DefaultRequestCulture = new RequestCulture("en"),
//    SupportedCultures = supportedCultures,
//    SupportedUICultures = supportedCultures
//});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.EnsureDatabaseCreated();
app.Run();

public class LanguageRouteConstraint : IRouteConstraint
{
    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
    {
        if (!values.ContainsKey("lang"))
            return false;

        string? culture = values["lang"]?.ToString();
        return culture == "en-US" || culture == "he-IL" || culture == "ru-RU";
    }
}


public class RouteDataRequestCultureProvider : RequestCultureProvider
{
    public int IndexOfCulture;

    public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));

        string? culture = httpContext.Request.Path.Value is not null && httpContext.Request.Path.Value.Split('/').Length > IndexOfCulture ? httpContext.Request.Path.Value?.Split('/')[IndexOfCulture]?.ToString() : null;

        ProviderCultureResult? providerResultCulture = culture is null ? null : new(culture);

        return Task.FromResult(providerResultCulture);
    }
}