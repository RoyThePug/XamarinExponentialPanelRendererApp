using ExpPanelRenderer.Domain.Service.TextStorage;
using ExpPanelRenderer.Domain.Tests.Base;
using ExpPanelRenderer.Domain.Tests.MockService;
using ExpPanelRenderer.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace ExpPanelRenderer.Domain.Tests.Test.ViewModel;

public class MainViewModelTests : BaseTest
{
    private ITextStorageService _mockTextStorageService;
    private MainViewModel _mainVm;

    [SetUp]
    public void SetUp()
    {
        _mockTextStorageService = ServiceCollection.ServiceProvider.GetRequiredService<ITextStorageService>();
        _mainVm = ServiceCollection.ServiceProvider.GetRequiredService<MainViewModel>();
    }

    [Test]
    public async Task GetTextCommand_Return_AllText()
    {
        //Act
        await _mainVm.GetAllTextCommand.ExecuteAsync(null).ConfigureAwait(false);
        //Assert
        Assert.NotNull(_mainVm.TextItems);
        Assert.IsTrue(_mainVm.TextItems.Any());
    }

    [Test]
    [TestCase("text")]
    public async Task SearchTextCommand_Return_Success(string searchText)
    {
        //Arrange
        var expectedResults = new List<string>
        {
            "For many centuries",
            "random text",
            "and philosophical texts",
            "the basis of which",
            "text",
            "and texts",
            "For many centuries",
            "random text",
            "and philosophical texts",
        };
        //Act
        foreach (var expectedItem in expectedResults)
        {
            await _mainVm.SearchTextCommand.ExecuteAsync(searchText).ConfigureAwait(false);
            //Assert
            Assert.IsTrue(expectedItem.Equals(_mainVm.CurrentResultText));
        }

    }

    [Test]
    [TestCase("...")]
    public async Task SearchTextCommand_Return_EmptyResult(string searchText)
    {
        //Arrange
        await _mockTextStorageService.GetAllText();
        //Act
        await _mainVm.SearchTextCommand.ExecuteAsync(searchText).ConfigureAwait(false);
        var actualFirst = _mainVm.CurrentSearchText;
        //Assert
        Assert.IsEmpty(actualFirst);
    }

    [Test]
    [TestCase("Some Text", true)]
    [TestCase("", false)]
    public void CheckSearchTextIsEmpty_Returns_SearchCommandCanExecute(string searchText, bool expectedResult)
    {
        //Arrange
        _mainVm.CurrentSearchText = searchText;
        //Act
        var canSearch = _mainVm.SearchTextCommand.CanExecute(null);
        //Assert
        Assert.IsTrue(canSearch.Equals(expectedResult));
    }
}