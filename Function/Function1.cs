using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Function
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var postData = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var authHeader = req.Headers["Authorization"].FirstOrDefault();

            log.LogInformation($"Headers: {req.Headers["Authorization"]}");
            log.LogInformation($"Data: {postData.ReadAsStringAsync()}");

            using var client = new HttpClient();
            client.BaseAddress = new Uri(req.Headers["EndpointURI"]);
            client.DefaultRequestHeaders.Add("Authorization", authHeader);

            var result = await client.PostAsync("", postData);
            var response = await result.Content.ReadAsStringAsync();

            log.LogInformation(response);
            
            return new OkObjectResult(response);
        }
    }
}
