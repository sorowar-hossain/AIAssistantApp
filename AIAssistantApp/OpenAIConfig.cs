using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssistantApp
{
    public static class OpenAIConfig
    {
        /*
            we can set this Environment Variable's value using Windows PowerShell command,
            because  anyone can not see the secret key
         */
        public static string ApiKey { get; } =
            Environment.GetEnvironmentVariable("OPENAI_API_KEY")
            ?? throw new Exception("OpenAI API key not found.");

        public static string connectionString =
    "Server=YOUR_SERVER;Database=YOUR_DATABASE;Trusted_Connection=True;TrustServerCertificate=True;";
    }
}
