using AIAssistantApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssistantApp.IServices
{
    public interface ILLMService
    {
        Task<string> GenerateEmailAsync(EmailRequest request); 
    }
}
