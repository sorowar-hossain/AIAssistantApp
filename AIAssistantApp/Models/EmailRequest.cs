using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssistantApp.Models
{
    public class EmailRequest
    {
        public string Purpose { get; set; } = "";
        public string Recipient { get; set; } = "";
        public string Reason { get; set; } = "";
        public string Duration { get; set; } = "";
        public string Tone { get; set; } = ""; // such as Professional, formal, friendly
    }
}
