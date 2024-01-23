using IMS.Plugins.InMemory;
using IMS.UseCases.Inventories;
using IMS.UseCases.PluginInterfaces;
using IMS.UseCases.Products;
using IMS.WebApp.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

// registered the service for all the pipeline middleware
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

// AddSingleton, AddTransient is a lifetime management to allow the framework to provide the instance of class whenever needed
//  and also use to indicate how long these instance will live

// whenever the usecase is require by the blazor application then it will see that the usecase class is require the IInventoryRepository
//  interface with implementation by InventoryRepository and it will find that the it's already has the interface then it will not
//  create a new instance and provide that already create instance into the instructure of the usecase class and use it
// AddSingleton indicate that when the application require the instance of the class it will create and store the instance in the application
//  and will stay when the application is running
//  when mutiple user it will send the same instance to use
builder.Services.AddSingleton<IInventoryRepository, InventoryRepository>(); // mapping the interface with the implementation
builder.Services.AddSingleton<IProductRepository, ProductRepository>();

// when component is initiailize the framework will provide the instance of ViewInventoriesByNameUseCase class
// Transient indicate that whenever we require the instance of the class
//  the frame work will not store the instance of the class anywhere
// when mutiple user it will send it own copy to the user
builder.Services.AddTransient<IViewInventoriesByNameUseCase, ViewInventoriesByNameUseCase>(); // map <abstraction, implementations>

// AddScoped the instance of the class is store as long as the life time of signalr and will be reuse the same instance with it's lifetime
//  example with when reload the web it's will disconnect the signalr 

builder.Services.AddTransient<IAddInventoryUseCase, AddInventoryUseCase>();
builder.Services.AddTransient<IEditInventoryUseCase, EditInventoryUseCase>();
builder.Services.AddTransient<IViewInventoryByIdUseCase, ViewInventoryByIdUseCase>();

builder.Services.AddTransient<IViewProductsByNameUseCase, ViewProductsByNameUseCase>();
builder.Services.AddTransient<IAddProductUseCase, AddProductUseCase>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
