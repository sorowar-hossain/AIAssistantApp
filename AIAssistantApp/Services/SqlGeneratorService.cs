using AIAssistantApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssistantApp.Services
{
    public class SqlGeneratorService
    {
        private readonly OpenAIService openAIService;
        public SqlGeneratorService(OpenAIService openAIService)
        {
            this.openAIService = openAIService;
        }
  
        public async Task<string> GenerateSqlQuery(DatabaseSchema schema, string userQuestion)
        {

            var prompt = await BuildPrompt(schema, userQuestion);
            Console.WriteLine(prompt.ToString());
            var generatedSql = await openAIService.GenerateResponseAsync(prompt);
            return generatedSql;
        }
        public async Task<string> BuildPrompt(DatabaseSchema schema, string userQuestion)
        {
            var prompt = new StringBuilder();

            prompt.AppendLine("You are an expert SQL Server query generator.");
            prompt.AppendLine();
            prompt.AppendLine("Generate a SQL Server query based on the database schema and user question.");
            prompt.AppendLine();
            prompt.AppendLine("DATABASE SCHEMA:");

            foreach (var table in schema.Tables)
            {
                prompt.AppendLine($"Table: {table.TableName}");

                foreach (var column in table.Columns)
                {
                    prompt.AppendLine($"- {column}");
                }

                prompt.AppendLine();
            }

            prompt.AppendLine("USER QUESTION:");
            prompt.AppendLine(userQuestion);
            prompt.AppendLine();

            prompt.AppendLine("RULES:");
            prompt.AppendLine("- Generate only SQL.");
            prompt.AppendLine("- Use SQL Server syntax.");
            prompt.AppendLine("- Do not explain the query.");
            prompt.AppendLine("- Do not invent tables or columns.");

           
            return prompt.ToString();
        }
    }
}
