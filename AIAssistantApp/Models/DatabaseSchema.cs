using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssistantApp.Models
{
    public class DatabaseSchema
    {
        public List<TableSchema> Tables { get; set; } = new();
    }
}
