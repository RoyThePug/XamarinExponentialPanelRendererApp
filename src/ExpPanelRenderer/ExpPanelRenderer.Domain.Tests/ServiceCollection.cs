using ExpPanelRenderer.Domain.Service.TextStorage;
using ExpPanelRenderer.Domain.Tests.MockService;
using ExpPanelRenderer.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace ExpPanelRenderer.Domain.Tests;

public static class ServiceCollection
{
    private static IServiceProvider _serviceProvider;

    public static IServiceProvider ServiceProvider => _serviceProvider;

    public static void Initialize() => _serviceProvider = CreateContainer();

    private static IServiceProvider CreateContainer()
    {
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        services.AddSingleton<ITextStorageService, MockTextStorageService>();
        services.AddSingleton<MainViewModel>();
        
        return services.BuildServiceProvider();
    }
}