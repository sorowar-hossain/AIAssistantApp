using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssistantApp.Models
{
    public class TableSchema
    {
        public string TableName { get; set; } = string.Empty;
        public List<string> Columns { get; set; } = new();
    }
}
