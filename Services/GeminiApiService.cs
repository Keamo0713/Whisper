// Whisper/Services/GeminiApiService.cs
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration; // Needed for IConfiguration
using System.Collections.Generic; // Needed for List

namespace Whisper.Services
{
    public class GeminiApiService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        // Constructor to inject IConfiguration and HttpClient
        public GeminiApiService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<string> GetAdvice(string confessionText)
        {
            // --- Google Gemini API Integration ---
            // Ensure you have configured your appsettings.json with this value.
            string apiKey = _configuration["Gemini:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("Gemini API key not configured in appsettings.json.");
                return "I am unable to provide advice at this moment. Please ensure Gemini API configuration is complete.";
            }

            try
            {
                // Construct the chat history for the Gemini API
                var chatHistory = new List<object>
                {
                    new { role = "user", parts = new[] { new { text = $"Provide concise and empathetic advice for the following confession: '{confessionText}'" } } }
                };

                var payload = new
                {
                    contents = chatHistory,
                    // Optional: You can add generationConfig here if you need structured responses or specific model parameters
                    // generationConfig = new {
                    //     temperature = 0.7,
                    //     maxOutputTokens = 150
                    // }
                };

                var jsonContent = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // The API key is passed as a query parameter for the Gemini API
                // No need to add it to DefaultRequestHeaders for this specific API pattern
                var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

                var response = await _httpClient.PostAsync(apiUrl, content);

                response.EnsureSuccessStatusCode(); // Throws an exception if the HTTP response status is an error code.

                var responseBody = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(responseBody);

                // Extracting the advice from the Gemini API response structure
                // Path: candidates[0].content.parts[0].text
                var advice = jsonDoc.RootElement
                                    .GetProperty("candidates")[0]
                                    .GetProperty("content")
                                    .GetProperty("parts")[0]
                                    .GetProperty("text")
                                    .GetString() ?? "No advice received from AI.";
                return advice;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"API Request Exception: {e.Message}");
                return $"There was an issue connecting to the Gemini AI service: {e.Message}. Please check your network and API configuration.";
            }
            catch (JsonException e)
            {
                Console.WriteLine($"JSON Deserialization Exception: {e.Message}");
                return $"There was an issue processing the AI's response: {e.Message}. The API response might be in an unexpected format.";
            }
            catch (Exception e)
            {
                Console.WriteLine($"An unexpected error occurred while getting advice: {e.Message}");
                return $"An unexpected error occurred: {e.Message}. Please try again later.";
            }
        }
    }
}