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
    [TestCase("texts")]
    public async Task SearchTextCommand_Return_Success(string searchText)
    {
        //Arrange
        var expectedFirst = "and philosophical texts";
        var expectedSecond = "and texts";
        await _mockTextStorageService.GetAllText();
        //Act
        await _mainVm.SearchTextCommandCommand.ExecuteAsync(searchText).ConfigureAwait(false);
        var actualFirst = _mainVm.CurrentSearchText;
        await _mainVm.SearchTextCommandCommand.ExecuteAsync(searchText).ConfigureAwait(false);
        var actualSecond = _mainVm.CurrentSearchText;
        //Assert
        Assert.IsTrue(expectedFirst.Equals(actualFirst));
        Assert.IsTrue(expectedSecond.Equals(actualSecond));
    }
    
    [Test]
    [TestCase("...")]
    public async Task SearchTextCommand_Return_EmptyResult(string searchText)
    {
        //Arrange
        var expectedFirst = "...";
        await _mockTextStorageService.GetAllText();
        //Act
        await _mainVm.SearchTextCommandCommand.ExecuteAsync(searchText).ConfigureAwait(false);
        var actualFirst = _mainVm.CurrentSearchText;
        //Assert
        Assert.IsEmpty(actualFirst);
    }

    [Test]
    public void GetText_ThrowsException()
    {
    }
}