using MauiApp1.Services;
using MauiApp1.Shared.Services;
using Microsoft.Extensions.Logging;

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

            // Add device-specific services used by the MauiApp1.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif
            // Register HttpClient for MAUI (calls localhost from emulator/device needs 10.0.2.2 for Android)
            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri("https://10.0.2.2:44323/api/") // Android emulator
                // BaseAddress = new Uri("https://localhost:44323/api/") // Windows/Mac
            });
            return builder.Build();
        }
    }
}
