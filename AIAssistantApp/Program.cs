

using AIAssistantApp;
using AIAssistantApp.Models;
using AIAssistantApp.Services;

/*
var request = new EmailRequest
{
    Purpose = "Leave Request",
    Recipient = "HR Manager",
    Reason = "Family event",
    Duration = "3 days",
    Tone = "Professional"
};
EmailGeneratorService service = new EmailGeneratorService(new OpenAIService());
var responsemail = service.GenerateEmailAsync(request);
Console.WriteLine(responsemail);
*/




var schema = new DatabaseSchema
{
    Tables =
    {
        new TableSchema
        {
            TableName = "Employees",
            Columns =
            {
                "Id",
                "Name",
                "Department",
                "Salary",
                "JoiningDate"
            }
        }
    }
};



/* // 
var userQuestion =
    "Show all employees who work in the IT department.";
var sqlGeneratorService = new SqlGeneratorService(new OpenAIService());

var response = sqlGeneratorService.GenerateSqlQuery(
    schema,
    userQuestion);

Console.WriteLine("========== PROMPT ==========");
Console.WriteLine(response);
*/

var userQuestion ="how many employee work in the IT department.";
SqlExecutionService sqlExecutionService = new SqlExecutionService(OpenAIConfig.connectionString);
var databaseAssistant = new DatabaseAssistantService(new SqlGeneratorService(new OpenAIService()), sqlExecutionService);
var response = await databaseAssistant.ExecuteUserQuestionAsync(schema,userQuestion);
Console.WriteLine(response);


Console.ReadKey();