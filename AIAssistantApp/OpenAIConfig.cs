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
            we can set this value using Windows PowerShell command,
            bcos any can not see the secret key
         */
        public static string ApiKey { get; } =
            Environment.GetEnvironmentVariable("OPENAI_API_KEY")
            ?? throw new Exception("OpenAI API key not found.");
    }
}
