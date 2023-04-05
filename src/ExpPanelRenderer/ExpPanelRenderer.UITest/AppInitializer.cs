using System;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace ExpPanelRenderer.UITest
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            if (platform == Platform.Android)
            {
                return ConfigureApp.Android
                                   .EnableLocalScreenshots()
                                   .ApkFile("D:/Workspace/Xamarin/XamarinExponentialPanelRendererApp/src/ExpPanelRenderer/ExpPanelRenderer.Android/bin/Debug/com.companyname.exppanelrenderer.apk")
                                   .StartApp();
            }

            return ConfigureApp.iOS.StartApp();
        }
    }
}