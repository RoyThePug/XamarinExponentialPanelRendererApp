using System;
using Microsoft.Extensions.DependencyInjection;

namespace ExpPanelRenderer;

public class Startup
{
    public static IServiceProvider ServiceProvider { get; set; }

    public static IServiceProvider Init()
    {
        var serviceProvider = new ServiceCollection()
                              .ConfigureService()
                              .ConfigureViewModel()
                              .ConfigureView()
                              .BuildServiceProvider();

        ServiceProvider = serviceProvider;

        return serviceProvider;
    }
}