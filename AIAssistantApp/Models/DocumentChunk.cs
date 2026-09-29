using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssistantApp.Models
{
    public class DocumentChunk
    {
        public int Id { get; set; }
        public int PageNumber { get; set; }
        public string SectionTitle { get; set; } = "";
        public string Text { get; set; } = "";
        public double[] Embedding { get; set; } = [];
    }
}
