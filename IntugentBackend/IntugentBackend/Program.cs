using IntugentBackend.Services.Core;
using IntugentBackend.Services.Data;
using IntugentBackend.Services.Rnd;
using IntugentBackend.Services.Mfg;
using IntugentBackend.Services.Admin;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Service Registration (Dependency Injection) ---
// Each class is registered individually so a controller only causes
// construction of the specific classes it actually depends on, instead of
// one god-object graph being built for every request.

// Tier 0 — no dependencies
builder.Services.AddSingleton<CDefualts>();
builder.Services.AddSingleton<CLists>();
builder.Services.AddSingleton<CNNModel>();
builder.Services.AddSingleton<cMatrix>();
builder.Services.AddSingleton<CAppParam>();
builder.Services.AddSingleton<CForm>();
builder.Services.AddSingleton<CForms>();
builder.Services.AddSingleton<CJetMix>();
builder.Services.AddSingleton<CMaterial>();
builder.Services.AddSingleton<Params>();
builder.Services.AddSingleton<CRCalc>();
builder.Services.AddSingleton<CRData>();
builder.Services.AddSingleton<CUConv>();
builder.Services.AddSingleton<RNDProperties>();
builder.Services.AddSingleton<RNDRawProps>();
builder.Services.AddSingleton<RNDTDRV>();
builder.Services.AddSingleton<AIModel>();
builder.Services.AddSingleton<SessionState>();

// Tier 1 — Cbfile (owns the SQL connection, now takes IConfiguration instead of a static field)
builder.Services.AddSingleton<Cbfile>();

// Tier 2 — depend on Cbfile only
builder.Services.AddSingleton<CDBase>();
builder.Services.AddSingleton<CNNData>();
builder.Services.AddSingleton<MfgDimStability>();
builder.Services.AddSingleton<MfgFinishedGoods>();
builder.Services.AddSingleton<MfgAdmin>();
builder.Services.AddSingleton<MfgInProcess>();

// Tier 2b — Cbfile + CDefualts
builder.Services.AddSingleton<CProdTargets>();
builder.Services.AddSingleton<CAnalysisData>();
builder.Services.AddSingleton<MfgProcessCheck>();
builder.Services.AddSingleton<MfgReports>();

// Tier 2c — CLists only
builder.Services.AddSingleton<RNDRValues>();
builder.Services.AddSingleton<MfgJetMixing>();

// Tier 3 — Cbfile + CDefualts + CLists
builder.Services.AddSingleton<MfgHome>();
builder.Services.AddSingleton<RNDHome>();
builder.Services.AddSingleton<MfgPlantData>();

// Tier 4 — depend on Tier 2/3 outputs
builder.Services.AddSingleton<CIPProdTargets>();
builder.Services.AddSingleton<RNDFormulations>();

// Tier 5 — MainWindow (session bootstrap; mutates Cbfile/CDefualts/CLists in place)
builder.Services.AddSingleton<MainWindow>();

// Domain-specific façade services
builder.Services.AddScoped<RNDRValuesService>();
builder.Services.AddScoped<RndPropertiesService>();
builder.Services.AddScoped<RNDTDRVService>();
builder.Services.AddScoped<RndRawPropsService>();
builder.Services.AddScoped<MfgAnalysisService>();
builder.Services.AddScoped<FormulationsPageService>();

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