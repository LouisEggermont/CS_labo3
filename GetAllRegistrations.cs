using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.SqlClient;
using Company.Function.Models;
using Azure.Identity;


namespace Company.Function
{
    public static class GetAllRegistrations
    {
        [FunctionName("GetAllRegistrations")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/registrations")] HttpRequest req,
            ILogger log)
        {
         try
            {
                List<Registration> registrations = new List<Registration>();

                // Use DefaultAzureCredential to get a token
                var credential = new DefaultAzureCredential();
                var token = credential.GetToken(new Azure.Core.TokenRequestContext(new[] { "https://database.windows.net/.default" }));

                string connectionString = "Server=tcp:labo3.database.windows.net,1433;Initial Catalog=labo3;TrustServerCertificate=False";

                // using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("connectionString")))
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.AccessToken = token.Token;
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "select * from registrations";
                        command.CommandText = sql;

                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            // days.Add(reader["DagVanDeWeek"].ToString());
                            Guid RegistrationId = (Guid)reader["RegistrationId"];
                            string LastName = reader["LastName"].ToString();
                            string FirstName = reader["FirstName"].ToString();
                            string Email = reader["Email"].ToString();
                            string Zipcode = reader["Zipcode"].ToString();
                            int Age = int.Parse(reader["Age"].ToString());
                            bool IsFirstTimer = Convert.ToBoolean(reader["IsFirstTimer"]);
                            Registration newRegistration = new Registration{
                                RegistrationId = RegistrationId,
                                LastName = LastName,
                                FirstName = FirstName,
                                Email = Email,
                                Zipcode = Zipcode,
                                Age = Age,
                                IsFirstTimer = IsFirstTimer
                            };
                            registrations.Add(newRegistration);
                        }




                    }
                }

                return new OkObjectResult(registrations);
            }
            catch (Exception ex)
            {
                // Log the exception

                log.LogError(ex, "An error occurred while processing the request.");

                throw new ApplicationException("An unexpected error occurred. Please try again later or contact support.", ex);

                // Return an appropriate error response
                // return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
