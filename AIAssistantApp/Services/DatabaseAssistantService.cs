using AIAssistantApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssistantApp.Services
{
    /*
            We'll build it in stages:

            1. User asks a question
                      ↓
            2. Get database schema
                      ↓
            3. Generate SQL
                      ↓
            4. Validate SQL
                      ↓
            5. Execute SQL Server
                      ↓
            6. Get result
                      ↓
            7. LLM converts result into a natural-language answer
    
    For example:
        User: How many employees are in IT?
    
    Then the assistant responds:
        There are 3 employees in the IT department.
     */
    public class DatabaseAssistantService
    {
        private readonly SqlGeneratorService sqlGeneratorService;
        private readonly SqlExecutionService sqlExecutionService;

        public DatabaseAssistantService(SqlGeneratorService sqlGeneratorService, SqlExecutionService sqlExecutionService)
        {
            this.sqlGeneratorService = sqlGeneratorService;
            this.sqlExecutionService = sqlExecutionService;
        }

        public async Task<string> ExecuteUserQuestionAsync(
     DatabaseSchema schema,
     string userQuestion)
        {
            try
            {
                var sql = await sqlGeneratorService.GenerateSqlQuery(
                    schema,
                    userQuestion);

                if (!ValidateSql(sql))
                {
                    return "You have no permission to execute this query.";
                }

                var table = await sqlExecutionService.ExecuteQueryAsync(sql);

                return ConvertDataTableToText(table);
            }
            catch (Exception ex)
            {
                return $"Unable to execute the query. Error: {ex.Message}";
            }
        }

        public string ConvertDataTableToText(DataTable table)
        {
            if (table.Rows.Count == 0)
            {
                return "No records found.";
            }

            var result = new StringBuilder();
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    result.AppendLine(
                                $"{column.ColumnName}: {row[column]}");
                   
                }
                result.AppendLine("------");
            }

            return result.ToString();
        }

        public bool ValidateSql(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return false;

            sql = sql.Trim();

            // Only allow SELECT queries
            if (!sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                return false;

            // Reject dangerous commands
            string[] forbiddenKeywords =
            {
                "INSERT",
                "UPDATE",
                "DELETE",
                "DROP",
                "ALTER",
                "TRUNCATE",
                "CREATE"
            };

            foreach (var keyword in forbiddenKeywords)
            {
                if (sql.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return true;
        }
    }
}
