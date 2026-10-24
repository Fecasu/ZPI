using WebStore.DAL.EF;
using WebStore.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWebStore(builder.Configuration.GetConnectionString("WebStore")
    ?? ApplicationDbContextFactory.LocalConnection);
var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");
app.Run();
