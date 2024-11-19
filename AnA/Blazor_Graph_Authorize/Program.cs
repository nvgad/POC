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
using Microsoft.AspNetCore.Authentication;

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
var Instance = builder.Configuration.GetValue<string>("AzureAd:Instance");
var Domain = builder.Configuration.GetValue<string>("AzureAd:Domain");
var CallbackPath = builder.Configuration.GetValue<string>("AzureAd:CallbackPath");

//builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);
builder.Services.AddAuthentication()
.AddMicrosoftIdentityWebApp(options =>
{
    options.ClientSecret = ClientSecret;
    options.ClientId = ClientId;
    options.TenantId = TenantId;
    options.Instance = Instance;
    options.Domain = Domain;
    options.CallbackPath = CallbackPath;
}, cookieScheme: null)
    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
        .AddMicrosoftGraph(builder.Configuration.GetSection("DownstreamApi"))
        .AddInMemoryTokenCaches();


//var scopes = new[] { ".default" };
//var scopes = new[] { "user.read" };
//var clientSecretCredential = new ClientSecretCredential(
//TenantId, ClientId, ClientSecret);
//var graphClient = new GraphServiceClient(clientSecretCredential, scopes);
//builder.Services.AddSingleton(graphClient);


//builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme,
//                        options =>
//                        {
//                            options.ClientId = ClientId;
//                            options.ClientSecret = ClientSecret;
//                            options.Authority = "https://login.microsoftonline.com/" + TenantId + "/v2.0/";
//                        });

//builder.Configuration.Bind("AzureAd");
//builder.Configuration.Bind("ClientId", ClientId);
//builder.Configuration.Bind("TenantId", TenantId);
//builder.Configuration.Bind("ClientSecret", ClientSecret);




//builder.Services.AddAuthorization(options =>
//{
//    options.FallbackPolicy = new AuthorizationPolicyBuilder()
//        .RequireAuthenticatedUser().Build();
//});

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


