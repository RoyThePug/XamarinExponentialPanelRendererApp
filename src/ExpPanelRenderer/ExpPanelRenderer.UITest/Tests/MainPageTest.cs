using NUnit.Framework;
using Xamarin.UITest;

namespace ExpPanelRenderer.UITest.Tests;

public class MainPageTest : BaseTest
{
    public MainPageTest(Platform platform) : base(platform)
    {
    }

    [Test]
    public void EnterSearchTestAndSearch()
    {
        //Act
        MainPage.EnterSearchText("f");

        MainPage.PressSearchButton();
    }

    [Test]
    public void AddVisualItem()
    {
        //Act
        MainPage.PressAddVisualItemButton();
        
        MainPage.PressAddVisualItemButton();
        
        MainPage.PressAddVisualItemButton();
    }
}