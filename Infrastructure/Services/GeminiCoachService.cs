using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class GeminiCoachService : ICoachService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GeminiCoachService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> GetStudyTipsAsync(string courseTitle, string courseDescription)
    {
        var rawKey = _configuration["Gemini:ApiKey"] 
                     ?? _configuration["Gemini__ApiKey"] 
                     ?? Environment.GetEnvironmentVariable("Gemini__ApiKey");
                     
        var apiKey = rawKey?.Trim();
        
        if (string.IsNullOrEmpty(apiKey))
        {
            return "Gemini API key is not configured. Please set the Gemini__ApiKey environment variable.";
        }

        var url = $"https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash:generateContent?key={apiKey}";

        var prompt = $@"Act as a professional technical coach. 
The user is studying a course titled '{courseTitle}'.
Course description: '{courseDescription}'.
Provide 3 concise, actionable study tips or mini-projects the user can do to master this specific topic faster. Keep it under 150 words total and format it cleanly with bullet points. Always reply in English.";

        var payload = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to get tips from Gemini API. Status: {response.StatusCode}. Details: {errorContent}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(jsonResponse);
        
        try
        {
            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();
                
            return text ?? "No tips generated.";
        }
        catch
        {
            return "Unable to parse response from Gemini.";
        }
    }
}
