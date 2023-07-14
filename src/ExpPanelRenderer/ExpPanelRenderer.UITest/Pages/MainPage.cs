using System;
using System.Linq;
using Xamarin.UITest;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace ExpPanelRenderer.UITest.Pages;

public class MainPage : BasePage
{
    private readonly Query _addVisualBtn, _searchBtn, _searchTextEntry, _wordItemsControl;

    public MainPage(IApp app, string pageTitle) : base(app, pageTitle)
    {
        _addVisualBtn = GenerateMarkedQuery("btnAddFigure");
        
        _searchBtn = GenerateMarkedQuery("searchBtn");

        _searchTextEntry = GenerateMarkedQuery("SearchTextEntry");

        _wordItemsControl = GenerateMarkedQuery("wordItemsControl");
    }

    public void EnterSearchText(string searchText)
    {
        EnterText(_searchTextEntry, searchText);

        App.PressEnter();
    }

    public void PressSearchButton()
    {
        App.Tap(_searchBtn);
    }

    public void PressAddVisualItemButton()
    {
        App.Tap(_addVisualBtn);
    }
}