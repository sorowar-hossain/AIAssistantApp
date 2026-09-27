using AIAssistantApp.Models;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssistantApp.Services
{
    public class EmailGeneratorService 
    {

        private readonly OpenAIService openAIService;
        public EmailGeneratorService(OpenAIService openAIService)
        {
            this.openAIService = openAIService; 
        }
        public async Task<string> GenerateEmailAsync(EmailRequest request)
        {
            try
            {
                string prompt = await BuildPrompt(request);
                var generatedEmail = await openAIService.GenerateResponseAsync(prompt);

                return generatedEmail;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating email: {ex.Message}");
                return string.Empty;
            }
        }

        // because the LLM can not understand the object but text
        public async Task<string> BuildPrompt(EmailRequest request)
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
