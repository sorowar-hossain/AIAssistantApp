

using AIAssistantApp.Models;
using AIAssistantApp.Services;


var request = new EmailRequest
{
    Purpose = "Leave Request",
    Recipient = "HR Manager",
    Reason = "Family event",
    Duration = "3 days",
    Tone = "Professional"
};
OpenAIService service = new OpenAIService();
var responsemail = service.GenerateEmailAsync(request);
Console.WriteLine(responsemail);


Console.ReadKey();