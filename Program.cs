var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Read the connection string from appsettings.json
string connectionString = builder.Configuration.GetConnectionString("AdventureWorksConnection");

// Register the EmployeeDAL as a Singleton or Transient service so it can be injected into Controllers
builder.Services.AddTransient<Portal_de_Consulta_de_Recursos_Humanos_AdventureWorks.DAL.EmployeeDAL>(provider => new Portal_de_Consulta_de_Recursos_Humanos_AdventureWorks.DAL.EmployeeDAL(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employee}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
