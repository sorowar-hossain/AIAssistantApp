using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssistantApp.Services
{
    public class SqlExecutionService
    {
        private readonly string _connectionString;
        public SqlExecutionService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<DataTable> ExecuteQueryAsync(string sql)
        {
            try
            {
                var dataTable = new DataTable();

                await using var connection =
                    new SqlConnection(_connectionString);

                await connection.OpenAsync();

                await using var command =
                    new SqlCommand(sql, connection);

                await using var reader =
                    await command.ExecuteReaderAsync();

                dataTable.Load(reader);

                return dataTable;
            }

            catch (Exception ex) 
            { 
                throw ex;
            }
           
        }
    }
}
