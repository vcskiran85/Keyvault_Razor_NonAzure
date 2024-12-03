using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System.Security.Cryptography.X509Certificates;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Retrieve Key Vault configuration settings and create a SecretClient instance for accessing secrets in Azure Key Vault.
var configuration = builder.Configuration;
var AzureADCertThumbprint = configuration["KeyVault:AzureADCertThumbprint"] ?? "";
var clientId = configuration["KeyVault:ClientId"] ?? "";
var clientSecret = configuration["KeyVault:ClientSecret"] ?? "";
var tenantId = configuration["KeyVault:TenantId"] ?? "";
var vaultUri = configuration["KeyVault:VaultUri"] ?? "";

using var x509Store = new X509Store(StoreLocation.CurrentUser);

x509Store.Open(OpenFlags.ReadOnly);

var x509Certificate = x509Store.Certificates
    .Find(
        X509FindType.FindByThumbprint,
        AzureADCertThumbprint,
        validOnly: false)
    .OfType<X509Certificate2>()
    .Single();


//var client = new SecretClient(new Uri(vaultUri), new ClientSecretCredential(tenantId, clientId, clientSecret));
var client = new SecretClient(new Uri(vaultUri), new ClientCertificateCredential(tenantId, clientId, x509Certificate));


builder.Services.AddSingleton(client);

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

app.UseAuthorization();

app.MapRazorPages();

app.Run();
