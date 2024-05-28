using AnA_MultiTenant.Components;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Graph;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph.Models.ExternalConnectors;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Azure.Identity;
using static System.Formats.Asn1.AsnWriter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//string[]? initialScopes = builder.Configuration.GetValue<string>("DownstreamApi:Scopes")?.Split(' ');
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration);

//builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
//    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
//    .AddMicrosoftGraph(builder.Configuration.GetSection("DownstreamApi"))
//    .AddInMemoryTokenCaches();
//string? tenantId = builder.Configuration.GetValue<string>("AzureAd:TenantId");
//string? clientId = builder.Configuration.GetValue<string>("AzureAd:ClientId");

//string? clientSecret = builder.Configuration.GetValue<string>("AzureAd:ClientSecret");
//var clientSecretCredential = new ClientSecretCredential(
//tenantId, clientId, clientSecret);
//var graphClient = new GraphServiceClient(clientSecretCredential, scopes);
//builder.Services.AddSingleton(graphClient);


//var scopes = new[] { ".default" };
//var baseUrl = string.Join("/",
//    builder.Configuration.GetSection("MicrosoftGraph")["BaseUrl"]);
//var scopes = builder.Configuration.GetSection("DownstreamApi:Scopes")
//    .Get<List<string>>();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();


//GraphServiceClient InitializeGraphClientAsync()
//{
//    var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

//    // Values from app registration
//    var clientId = MyConfig.GetValue<string>("AzureAd:ClientId");
//    var clientSecret = MyConfig.GetValue<string>("AzureAd:ClientSecret");

//    //var scopes = new[] { "https://graph.microsoft.com/.default" };
//    var scopes = new[] { ".default" };

//    //// Multi-tenant apps can use "common",
//    //// single-tenant apps must use the tenant ID from the Azure portal
//    var tenantId = MyConfig.GetValue<string>("AzureAd:TenantId");

//    //// https://learn.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
//    var clientSecretCredential = new ClientSecretCredential(
//        tenantId, clientId, clientSecret);

//    var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

//    return graphClient;
//}