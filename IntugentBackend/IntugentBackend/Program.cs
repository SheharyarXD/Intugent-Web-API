using IntugentBackend;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Rnd;
using IntugentBackend.Services.Mfg; 
using IntugentBackend.Services.Admin;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Service Registration (Dependency Injection) ---

// Core Services
builder.Services.AddSingleton<ObjectsService>();

// Domain-specific services
builder.Services.AddScoped<RNDRValuesService>();
builder.Services.AddScoped<RndPropertiesService>();
// ADD THIS LINE BELOW:
builder.Services.AddScoped<RNDTDRVService>();
builder.Services.AddScoped<RndRawPropsService>();
builder.Services.AddScoped<MfgAnalysisService>();

// Essential Framework Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
// ... later in Program.cs
// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// --- 2. Application Pipeline (Middleware) ---

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// IMPORTANT: CORS must come before Authorization and Controllers
app.UseCors("AllowAll");

app.UseAuthorization();

// This maps the controllers so the API endpoints actually work
app.MapControllers();

// This starts the Kestrel web server
app.Run();