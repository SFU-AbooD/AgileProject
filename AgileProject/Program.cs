using AgileProject.Migrations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie(
    options =>{options.LoginPath = "/Account/login"; })
    .AddFacebook(opt => {
        opt.AppId = "app";
        opt.AppSecret= "app";
        opt.SaveTokens = false;
        opt.Scope.Add("email");
        opt.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opt.Events = new OAuthEvents
        {
            OnCreatingTicket = context =>
            {
                Console.WriteLine(context);
                var claimsIdentity = new ClaimsIdentity(
                 context.Identity.Claims.ToList(),
                 CookieAuthenticationDefaults.AuthenticationScheme);
                 context.Principal = new ClaimsPrincipal(claimsIdentity);
                return Task.CompletedTask;
            }
        };

    });
builder.Services.AddDbContext<_dbcontext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("connection"));
});
var app = builder.Build();

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
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=signup}/{id?}");

app.Run();
