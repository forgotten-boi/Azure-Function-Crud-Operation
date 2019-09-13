using System;
using System.Threading.Tasks;
using AzureFunc.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;
using System.Security.Claims;

namespace AzureFunc
{
    public static class EmailScheduler
    {
        [FunctionName("EmailScheduler")]
        public static async Task Run([TimerTrigger("0 0 * * * *")]TimerInfo myTimer, ILogger log, ClaimsPrincipal claimsPrinciple)
        {
            try
            {
                log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
                //Send Email At each 24 hour

                var item = await CloudTableExtensions.GetAllObject(claimsPrinciple.Identity.Name);


                var json = Newtonsoft.Json.JsonConvert.SerializeObject(item);
                var emailSender = new EmailSender();
                await emailSender.SendEmailAsync(FunctionsSettings.ToEmail, "Azure function poc Data", json);
                log.LogInformation("Email Send");
            }
            catch (Exception ex)
            {

                throw ex;
            }
          
        }
    }
}
