using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using Company.Function.Models;

namespace Company.Function
{
    public static class AddRegistrations
    {
        [FunctionName("AddRegistrations")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route ="v1/registrations")] HttpRequest req,
            ILogger log)
        {
            // log.LogInformation("C# HTTP trigger function processed a request.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Registration registration = JsonConvert.DeserializeObject<Registration>(requestBody);
            // log.LogInformation(registration.ToString());
            using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("connectionString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "INSERT INTO registrations (RegistrationId, LastName, FirstName, Email, Zipcode, Age, IsFirstTimer) VALUES (NEWID(), @LastName, @FirstName, @Email, @Zipcode, @Age, @IsFirstTimer)";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@LastName", registration.LastName);
                        command.Parameters.AddWithValue("@FirstName", registration.FirstName);
                        command.Parameters.AddWithValue("@Email", registration.Email);
                        command.Parameters.AddWithValue("@Zipcode", registration.Zipcode);
                        command.Parameters.AddWithValue("@Age", registration.Age);
                        command.Parameters.AddWithValue("@IsFirstTimer", registration.IsFirstTimer);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        log.LogInformation($"rows affected: {rowsAffected}");
                    }
                }


            return new OkObjectResult(registration.ToString());
        }
    }
}
