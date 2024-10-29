using ClientSiteProductApiConsuming.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
namespace ClientSiteProductApiConsuming
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            Log.Logger = new LoggerConfiguration()
           .ReadFrom.Configuration(builder.Configuration)  // Read from appsettings.json
           .CreateLogger();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

           

            //Register HttpClient
            builder.Services.AddHttpClient();
          


            //Add session services to the container
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddCors(
                options =>
                {
                    options.AddPolicy("AllowAllOrigins",
                        builder =>
                        {
                            builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                        });
                });

            // Register IHttpContextAccessor to access HttpContext in controllers
            builder.Services.AddHttpContextAccessor();

            // Register application services
            builder.Services.AddTransient<IAuthService, AuthService>();
            builder.Services.AddTransient<IProductService, ProductService>();

            // Configure JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "https://myapi.example.com", // Set your valid issuer
                    ValidAudience = "https://myapp.example.com", // Set your valid audience
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("B2E2D9A18A4E54B17FC9AEF65B3B490715B8D36E7F1F11E1238D4A012D0E35D6")) // Set your secret key
                };
            });

            builder.Services.AddAuthorization(opt =>
            {
                opt.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
                opt.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
            });

            var app = builder.Build();

            // Add Razor Pages
            //builder.Services.AddRazorPages();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("AllowAllOrigins");

            //Enable session middleware before authentication and authorization
            app.UseSession();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
