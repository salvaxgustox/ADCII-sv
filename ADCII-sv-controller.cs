using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace umg.salva
{
    public static class ADCII_sv_controller
    {
        [FunctionName("ADCII_sv_controller")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string SqlConnectionString ="Server=tcp:umg.database.windows.net,1433;Initial Catalog=project;Persist Security Info=False;User ID=dbasalva;Password=Salva301190!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            SqlConnection conn = new SqlConnection(SqlConnectionString);
            conn.Open();
            string result = "...";
            log.LogInformation($"C# HTTP trigger function processed a request {req.Method}.");
            
            if (HttpMethods.IsPost(req.Method))
            {
                bool bomba = Boolean.Parse(req.Query["b"]);
                bool sensor = Boolean.Parse(req.Query["s"]);
                bool llave = Boolean.Parse(req.Query["l"]);
                string sql =  "INSERT INTO controller(bomba,sensor,llave,fecha) VALUES(@param1,@param2,@param3,@param4)";
                Console.WriteLine(sql);
                using(SqlCommand cmd = new SqlCommand(sql,conn)) 
                {
                    try
                    {
                        cmd.Parameters.Add("@param1", SqlDbType.Bit).Value = bomba;
                        cmd.Parameters.Add("@param2", SqlDbType.Bit).Value = sensor;
                        cmd.Parameters.Add("@param3", SqlDbType.Bit).Value = llave;
                        cmd.Parameters.Add("@param4", SqlDbType.DateTime).Value = DateTime.Now;
                        cmd.CommandType = CommandType.Text;
                        int query_rows_affected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"Rows affected: {query_rows_affected}");
                        result = "ok";
                    }
                    catch (System.Exception)
                    {
                        result = "Error en base de datos";
                    }
                    
                }
            } else if (HttpMethods.IsGet(req.Method)){
                string sql_command = "Select * from [dbo].[controller] order by fecha desc";
                    Console.WriteLine($"Sql command to be executed: {sql_command}");
                    SqlCommand command = new SqlCommand(sql_command, conn);
                    command.CommandType = System.Data.CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            Console.WriteLine($"Successfully Executed Azure Function at: {DateTime.Now}");
                            log.LogInformation($"Successfully Executed Azure Function at: {DateTime.Now}");
                            Console.WriteLine($"Row count processed: "+ reader.GetBoolean(1).ToString());
                            log.LogInformation($"Row count processed: "+ reader.GetBoolean(1).ToString());
                            result = $"{{b: {reader.GetBoolean(1)}, s: {reader.GetBoolean(2)}, l: {reader.GetBoolean(3)}, fecha: {reader.GetDateTime(4)} }}";
                        }
                    }
            }
            
            
            
            conn.Close();
            return new OkObjectResult(result);
        }
    }
}
