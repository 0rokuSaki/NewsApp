using NewsApp.Models;
using NewsApp.Settings;
using System.Net.Http.Json;

namespace NewsApp.Services;

public class NewsService : INewsService, IDisposable
{
    private bool _disposedValue;

    const string UriBase = "https://newsapi.org/v2";

    private readonly HttpClient httpClient = new()
    {
        BaseAddress = new(UriBase),
        DefaultRequestHeaders = { { "user-agent", "maui-projects-news/1.0"} }
    };

    private static string Headlines => $"{UriBase}/top-headlines?country=us&apiKey={ApiSettings.NewsApiKey}";
    private static string Global => $"{UriBase}/everything?q=global&apiKey={ApiSettings.NewsApiKey}";
    private static string Local => $"{UriBase}/everything?q=local&apiKey={ApiSettings.NewsApiKey}";

    public async Task<NewsResult> GetNews(NewsScope scope)
    {
        NewsResult result;
        string url = GetUrl(scope);

        try
        {
            result = await httpClient.GetFromJsonAsync<NewsResult>(url);
        }
        catch (Exception ex)
        {
            result = new()
            {
                Articles = new()
                {
                    new()
                    {
                        Title = $"HTTP Get failed: {ex.Message}",
                        PublishedAt = DateTime.Now,
                    }
                }
            };
        }
        return result!;
    }

    private string GetUrl(NewsScope scope)
    {
        return scope switch
        {
            NewsScope.Headlines => Headlines,
            NewsScope.Global => Global,
            NewsScope.Local => Local,
            _ => throw new InvalidOperationException("Undefined scope")
        };
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                httpClient.Dispose();
            }
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
