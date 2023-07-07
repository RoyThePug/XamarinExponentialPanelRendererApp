using ExpPanelRenderer.Domain.Service.TextStorage;

namespace ExpPanelRenderer.Domain.Service.TextStorage;

public class TextStorageService : ITextStorageService
{
    private IEnumerable<string> _textData;

    public async Task<IEnumerable<string>> GetAllText()
    {
        _textData = await Task.FromResult<IEnumerable<string>>(GetMockText());

        return _textData;
    }

    public async Task<string> SearchText(string subString, string currentSearchText)
    {
        var resultString = string.Empty;

        try
        {
            await Task.Run(() =>
            {
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

                return resultString;
            });
            return string.Empty;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    private IEnumerable<string> GetMockText()
    {
        yield return "For many centuries";
        yield return "random text";
        yield return "and philosophical texts";
        yield return "the basis of which";
        yield return "random text";
        yield return "and texts";
    }
}