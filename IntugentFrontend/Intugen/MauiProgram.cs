using Microsoft.Extensions.Logging;
using Intugen.Services;

namespace Intugen;

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

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddScoped<ApiService>();
        
        // Register HttpClient for API calls
        builder.Services.AddScoped(sp => 
            new HttpClient { BaseAddress = new Uri("https://localhost:7000/") }); // Your API base URL

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}