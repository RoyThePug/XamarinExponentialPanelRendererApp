using ExpPanelRenderer.Domain.Service.TextStorage;
using ExpPanelRenderer.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace ExpPanelRenderer;

public static class DependencyInjectionContainer
{
    public static IServiceCollection ConfigureService(this IServiceCollection services)
    {
        services.AddSingleton<ITextStorageService, TextStorageService>();

        return services;
    }

    public static IServiceCollection ConfigureViewModel(this IServiceCollection services)
    {
        services.AddSingleton<MainViewModel>();

        return services;
    }

    public static IServiceCollection ConfigureView(this IServiceCollection services)
    {
        services.AddTransient<MainPage>();

        return services;
    }
}