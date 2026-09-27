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
    public class LocalGemmaService
    {
        /*
         Gemma is a family of open AI language models developed by Google.
         
         Think of it like this:
         Gemma = a ready-made language model that can understand and generate text.
         
         */
       public async Task<string> GenerateEmailAsync(EmailRequest request)
        {
             
            try
            {
                string prompt = BuildPrompt(request);
                // Send prompt to local Gemma model
                // Receive generated response

                return "";
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
