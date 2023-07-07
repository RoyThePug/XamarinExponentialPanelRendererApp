namespace ExpPanelRenderer.Domain.Service.TextStorage
{
    public interface ITextStorageService
    {
        Task<IEnumerable<string>> GetAllText();
        Task<string> SearchText(string subString, string currentSearchText);
    }
}