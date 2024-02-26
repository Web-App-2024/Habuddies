using HaBuddies.Models;
using HaBuddies.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.Configure<HaBuddiesDatabaseSettings>(
    builder.Configuration.GetSection("HaBuddiesDatabase"));
builder.Services.AddSingleton<MongoService>();
builder.Services.AddSingleton<PostService>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
