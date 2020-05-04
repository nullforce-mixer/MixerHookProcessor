using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Nullforce.Mixer.Functions
{
    public static class OnHook
    {
        [FunctionName("OnHook")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Read the body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            // Retrieve the secret
            string secret = Environment.GetEnvironmentVariable("MIXER_CLIENT_SECRET");

            // Validate the request signature
            if (!IsRequestValid(req.Headers, secret, requestBody))
            {
                log.LogInformation("Unauthorized request received. Request signature was invalid.");
                return new UnauthorizedResult();
            }

            // TODO: Process the event
            log.LogInformation(requestBody);
            string name = req.Query["name"];

            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = "This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        private static bool IsRequestValid(IHeaderDictionary headers, string secret, string body)
        {
            var hmac = new HMACSHA384(Encoding.UTF8.GetBytes(secret));
            var hash = BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(body))).Replace("-", string.Empty);
            return headers["Poker-Signature"].Equals($"sha384={hash}");
        }

    }
}
