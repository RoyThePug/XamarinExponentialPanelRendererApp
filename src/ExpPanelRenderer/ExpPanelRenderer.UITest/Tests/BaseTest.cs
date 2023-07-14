using ExpPanelRenderer.UITest.Pages;
using NUnit.Framework;
using Xamarin.UITest;

namespace ExpPanelRenderer.UITest.Tests;

[TestFixture(Platform.Android)]
public abstract class BaseTest
{
    private IApp _app;
    private Platform _platform;
    protected MainPage MainPage;
    
    protected BaseTest(Platform platform)
    {
        _platform = platform;
    }

    [SetUp]
    public virtual void BeforeEachTest()
    {
        _app = AppInitializer.StartApp(_platform);

        MainPage = new MainPage(_app, "MainPage");
        
        _app.Screenshot("screen");
        
        MainPage.WaitForPageToLoad();
    }
}