using BankApp1;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ISignInService, SignInService>();

builder.Services.AddScoped<IStorageService, StorageService>();

builder.Services.AddBlazoredLocalStorage();
//var host = builder.Build();

//// One-time cleanup for old corrupted data
//var jsRuntime = host.Services.GetRequiredService<IJSRuntime>();
//await jsRuntime.InvokeVoidAsync("localStorage.clear");

//await host.RunAsync();


await builder.Build().RunAsync();
