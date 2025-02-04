using Blazor_Graph_Authorize.Components;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using System.IdentityModel.Tokens.Jwt;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
//using Azure.Core;
using System;
using Microsoft.Graph.Models;
using Microsoft.Extensions.Azure;
using Microsoft.Graph;
using Microsoft.Graph.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Graph.Models.ExternalConnectors;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web.TokenCacheProviders.Session;
using Microsoft.Identity.Web.TokenCacheProviders.Distributed;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var initialScopes = builder.Configuration.GetValue<string>("DownstreamApi:Scopes")?.Split(' ');

// This flag ensures that the ClaimsIdentity claims collection will be built from the claims in the token.
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

var keyVaultEndpoint = new Uri(builder.Configuration.GetValue<string>("AzureAd:SafeUri"));
var client = new SecretClient(keyVaultEndpoint, new DefaultAzureCredential());
var ClientId = client.GetSecret("ClientId").Value.Value;
var TenantId = client.GetSecret("TenantId").Value.Value;
var ClientSecret = client.GetSecret("ClientSecret").Value.Value;
//var ClientSecret = "hXh8Q~ggfstYsynMfk1zHRco2fkmeDI4xDDqIa5T";

//var Instance = builder.Configuration.GetValue<string>("AzureAd:Instance");
//var Domain = builder.Configuration.GetValue<string>("AzureAd:Domain");
//var CallbackPath = builder.Configuration.GetValue<string>("AzureAd:CallbackPath");




//var scopes = new[] { ".default" };
var scopes = initialScopes;
//var scopes = new[] { "user.read" };
var clientSecretCredential = new ClientSecretCredential(
TenantId, ClientId, ClientSecret);
var graphClient = new GraphServiceClient(clientSecretCredential, scopes);
builder.Services.AddSingleton(graphClient);


builder.Configuration.Bind("AzureAd");


var inMemorySettings = new Dictionary<string, string> {
   {"AzureAd:ClientId", ClientId },
    {"AzureAd:ClientSecret", ClientSecret },
    //{"AzureAd:ClientSecret", "hXh8Q~ggfstYsynMfk1zHRco2fkmeDI4xDDqIa5T" },

    {"AzureAd:TenantId", TenantId }
};


var configurationBuilder = new ConfigurationBuilder();
configurationBuilder.AddInMemoryCollection(inMemorySettings);
var configuration = configurationBuilder.Build();
builder.Configuration.AddConfiguration(configuration);

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration)
    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
    .AddMicrosoftGraph(builder.Configuration.GetSection("DownstreamApi"))
    .AddInMemoryTokenCaches();


builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});



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


app.UseRouting();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();


