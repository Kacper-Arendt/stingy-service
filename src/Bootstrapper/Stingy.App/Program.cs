using Azure.Identity;
using Sentry.Extensibility;
using Stingy.App.DI;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

if (builder.Environment.IsDevelopment())
    builder.Configuration.AddUserSecrets<Program>();

var keyVaultUrl = builder.Configuration["KeyVault"];
var c = builder.Environment.IsDevelopment();
builder.Configuration.AddAzureKeyVault(
    new Uri(keyVaultUrl),
    new DefaultAzureCredential());

builder.WebHost.UseSentry(o =>
{
    o.Dsn = builder.Configuration["Sentry:Dsn"];
    o.Debug = builder.Environment.IsDevelopment();
    o.TracesSampleRate = 1.0;
    o.AttachStacktrace = true;
    o.SendDefaultPii = true;
    o.MaxRequestBodySize = RequestSize.Always;
    o.MinimumBreadcrumbLevel = LogLevel.Debug;
    o.MinimumEventLevel = LogLevel.Warning;
    o.DiagnosticLevel = SentryLevel.Error;
});

builder.Services.AddAuthorization();
builder.Services.AddHealthChecks();
builder.Services.AddSignalR();
builder.RegisterModules();
    
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", corsBuilder =>
    {
        corsBuilder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
    options.AddPolicy("ProdCors", corsBuilder =>
    {
        corsBuilder.WithOrigins("")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors");
}
else
{
    app.UseHttpsRedirection();
    app.UseCors("ProdCors");
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// app.UseModules();
app.MapHealthChecks("/");
app.MapControllers();
// app.MapHub<RetroHub>("/hubs");

app.Run();