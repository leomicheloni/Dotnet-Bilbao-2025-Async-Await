using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;

namespace Demo;


public class Library
{
    public string Name { get; set; }
    public string Version { get; set; }
}

public class LibraryService
{
    private readonly HttpClient _client;

    public LibraryService(HttpClient client)
    {
        _client = client;
    }

    public async Task<IEnumerable<Library>> GetDataAsync()
    {
        var response = await _client.GetAsync("https://example.com");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStreamAsync();

        var libraries = await JsonSerializer.DeserializeAsync<List<Library>>(content);

        return libraries;
    }
}

