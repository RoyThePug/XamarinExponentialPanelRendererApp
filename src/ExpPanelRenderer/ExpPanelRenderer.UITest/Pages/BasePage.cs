using Xamarin.UITest;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace ExpPanelRenderer.UITest.Pages
{
    public abstract class BasePage
    {
        private readonly string _pageTitle;

        protected IApp App { get; }

        protected BasePage(IApp app, string pageTitle)
        {
            App = app;
            _pageTitle = pageTitle;
        }
        
        public virtual void WaitForPageToLoad() => App.WaitForElement(x => x.Marked(_pageTitle));
        
        protected static Query GenerateMarkedQuery(string automationId) => (x => x.Marked(automationId));
        
        protected void EnterText(Query textEntryQuery, string text)
        {
            App.ClearText(textEntryQuery);
            App.EnterText(textEntryQuery, text);
        }
    }
}