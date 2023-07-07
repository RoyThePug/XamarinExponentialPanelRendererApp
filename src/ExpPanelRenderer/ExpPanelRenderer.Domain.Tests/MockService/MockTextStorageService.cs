using ExpPanelRenderer.Domain.Service.TextStorage;

namespace ExpPanelRenderer.Domain.Tests.MockService;

public class MockTextStorageService : ITextStorageService
{
    #region Private
    
    private IEnumerable<string> _textData;
    
    #endregion
    
    public Task<IEnumerable<string>> GetAllText()
    {
        _textData = GetMockText();

        return Task.FromResult(_textData);
    }

    public Task<string> SearchText(string subString, string currentSearchText)
    {
        var resultString = string.Empty;
        
        var searchData = _textData.Where(x => x.Contains(subString));

        foreach (var text in searchData)
        {
            if (string.IsNullOrEmpty(currentSearchText))
            {
                resultString = text;
                
                break;
            }

            if (!text.Contains(currentSearchText))
            {
                resultString = text;
                
                break;
            }
        }

        return Task.FromResult(resultString);
    }

    #region Mock

    private IEnumerable<string> GetMockText()
    {
        yield return "For many centuries";
        yield return "random text";
        yield return "and philosophical texts";
        yield return "the basis of which";
        yield return "random text";
        yield return "and texts";
    }

    #endregion
}