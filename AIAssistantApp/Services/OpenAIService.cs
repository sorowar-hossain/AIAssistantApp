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
        private readonly ChatClient _chatClient;

        public OpenAIService()
        {
            _chatClient = new ChatClient(
            model: "gpt-4o-mini",
            apiKey: OpenAIConfig.ApiKey);
        }

        public async Task<string> GenerateResponseAsync(string prompt) 
        {
            ChatCompletion completion =
                await _chatClient.CompleteChatAsync(prompt);

            return completion.Content[0].Text;
        }
    }
}
