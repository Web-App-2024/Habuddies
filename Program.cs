using HaBuddies.Models;
using HaBuddies.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(builder => builder.AddConsole());
builder.Services.AddControllersWithViews();
builder.Services.Configure<HaBuddiesDatabaseSettings>(
    builder.Configuration.GetSection("HaBuddiesDatabase"));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSession();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<MongoService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<EventService>();

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

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Services.GetRequiredService<EventService>();

app.Run();
