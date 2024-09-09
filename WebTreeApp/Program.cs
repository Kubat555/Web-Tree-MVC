using Microsoft.EntityFrameworkCore;
using WebTreeApp;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<TreeDbContext>(op => op.UseNpgsql(builder.Configuration["PostgreDb"]));

var app = builder.Build();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tree}/{action=Index}/{id?}");

app.Run();
