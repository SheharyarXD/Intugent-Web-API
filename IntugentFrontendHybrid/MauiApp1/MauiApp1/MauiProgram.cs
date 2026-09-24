using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MauiApp1.Services;

namespace MauiApp1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // appsettings.json is embedded in the app assembly; it holds the backend API address.
            using (var settings = typeof(MauiProgram).Assembly.GetManifestResourceStream("MauiApp1.appsettings.json")
                ?? throw new InvalidOperationException("Embedded appsettings.json not found."))
            {
                builder.Configuration.AddJsonStream(settings);
            }
            var apiBaseUrl = builder.Configuration["Api:BaseUrl"]
                ?? throw new InvalidOperationException("Api:BaseUrl not found in appsettings.json.");

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSingleton<AppSessionState>();
            builder.Services.AddSingleton<LoadingState>();

            builder.Services.AddSingleton(sp =>
            {
                var handler = new HttpClientHandler();
                // Bypass cert validation in both Debug and Release: the backend always runs on the
                // same machine as this app (Kestrel's local HTTPS dev certificate is self-signed and
                // not trusted by default), and there's no network in between to protect against —
                // without this, a Release build would fail to connect on any machine that hasn't
                // separately run `dotnet dev-certs https --trust`.
                handler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                return new HttpClient(new LoadingHandler(sp.GetRequiredService<LoadingState>(), handler))
                {
                    BaseAddress = new Uri(apiBaseUrl)
                };
            });

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}