using AIAssistantApp.IServices;
using AIAssistantApp.Models;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssistantApp.Services
{
    public class OpenAIService : ILLMService
    {
        private readonly ChatClient _client;

        public OpenAIService()
        {
            _client = new ChatClient(
            model: "gpt-4o-mini",
            apiKey: OpenAIConfig.ApiKey);
        }

        public async Task<string> GenerateEmailAsync(EmailRequest request)
        {
            try
            {
                string prompt = BuildPrompt(request);
                ChatCompletion completion =
                    await _client.CompleteChatAsync(prompt);

                return completion.Content[0].Text;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating email: {ex.Message}");
                return string.Empty;
            }
        }

        // because the LLM can not understand the object but text
        public static string BuildPrompt(EmailRequest request)
        {
            return $"""
                Generate an email using the following information:

                Purpose: {request.Purpose}
                Recipient: {request.Recipient}
                Reason: {request.Reason}
                Duration: {request.Duration}
                Tone: {request.Tone}

                Return only the email.
                """;
        }
    }
}
