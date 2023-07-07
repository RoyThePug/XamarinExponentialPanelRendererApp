using ExpPanelRenderer.Domain.Service.TextStorage;

namespace ExpPanelRenderer.Domain.Service.TextStorage;

public class TextStorageService : ITextStorageService
{
    private IEnumerable<string> _textData;

    private List<string> _data;

    public TextStorageService()
    {
        _data = new List<string>();
    }

    public async Task<IEnumerable<string>> GetAllText()
    {
        _textData = await Task.FromResult(_data.Concat(GetMockText()));

        return _textData;
    }

    public async Task<string> SearchText(string subString, string currentResultText)
    {
        var resultString = string.Empty;

        try
        {
            _textData = await GetAllText();

            await Task.Run(() =>
            {
                var searchData = _textData.Where(x => x.Contains(subString, StringComparison.OrdinalIgnoreCase)).ToList();

                if (searchData.Count == 1)
                {
                    resultString = searchData.FirstOrDefault();
                }
                else
                {
                    var currentIndex = searchData.IndexOf(currentResultText);

                    if (string.IsNullOrEmpty(currentResultText) || currentIndex == searchData.Count - 1)
                    {
                        var result = searchData.Where(x => x.Contains(subString, StringComparison.OrdinalIgnoreCase)).ToList();

                        if (result.Any())
                        {
                            resultString = result.FirstOrDefault();
                        }
                    }
                    else
                    {
                        foreach (var text in searchData.Where(x => searchData.IndexOf(x) > currentIndex))
                        {
                            if (!text.Equals(currentResultText))
                            {
                                resultString = text;

                                break;
                            }
                        }
                    }
                }
            });
            return resultString;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public void AddText(string text)
    {
        _data.Add(text);
    }

    private IEnumerable<string> GetMockText()
    {
        yield return "For many centuries";
        // yield return "random text";
        // yield return "and philosophical texts";
        // yield return "the basis of which";
        // yield return "text";
        // yield return "and texts";
    }
}