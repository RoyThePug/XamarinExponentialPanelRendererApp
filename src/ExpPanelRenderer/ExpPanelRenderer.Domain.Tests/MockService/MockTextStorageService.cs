using ExpPanelRenderer.Domain.Service.TextStorage;
using Xamarin.Forms.Internals;

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

    public Task<string> SearchText(string subString, string currentResultText)
    {
        var resultString = string.Empty;

        try
        {
            _textData = GetMockText().ToList();
            
            var currentIndex = _textData.IndexOf(currentResultText);

            if (string.IsNullOrEmpty(currentResultText) || currentIndex == _textData.Count() - 1)
            {
                resultString = _textData.FirstOrDefault();
            }
            else
            {
                foreach (var text in _textData.Where(x => _textData.IndexOf(x) > currentIndex))
                {
                    if (!text.Equals(currentResultText))
                    {
                        resultString = text;

                        break;
                    }
                }
            }
        }
        catch (Exception)
        {
            return Task.FromResult(string.Empty);
        }

        return Task.FromResult(resultString);
    }

    public void AddText(string text)
    {
        throw new NotImplementedException();
    }

    public void RemoveText(string text)
    {
        throw new NotImplementedException();
    }

    #region Mock

    private IEnumerable<string> GetMockText()
    {
        yield return "For many centuries";
        yield return "random text";
        yield return "and philosophical texts";
        yield return "the basis of which";
        yield return "text";
        yield return "and texts";
    }

    #endregion
}