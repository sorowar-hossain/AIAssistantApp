

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

/*
    var userQuestion ="how many employee work in the IT department.";
    SqlExecutionService sqlExecutionService = new SqlExecutionService(OpenAIConfig.connectionString);
    var databaseAssistant = new DatabaseAssistantService(new SqlGeneratorService(new OpenAIService()), sqlExecutionService,new OpenAIService());
    var response = await databaseAssistant.ExecuteUserQuestionAsync(schema,userQuestion);
    Console.WriteLine(response);
*/

RagPipelineService ragPipelineService = 
                        new RagPipelineService(new PdfTextExtractorService(), 
                        new DocumentChunkService(), 
                        new RagEmbeddingService(), 
                        new RagVectorSearchService());
//var question = "How many days of annual leave are employees entitled to?";

var question = "What distance metric should be used for vector similarity searches, and which metric is explicitly banned?";

var response = ragPipelineService.RagSearch(
    "Emergency leave may be requested when an unexpected personal situation requires immediate absence. Employees should contact their manager as soon as possible when emergency leave is necessary"
    );
Console.ReadKey();