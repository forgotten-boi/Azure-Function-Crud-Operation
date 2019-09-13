using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using System;

namespace AzureFunc
{
    public class TelemetryClientFactory : ITelemetryClientFactory
    {
        public virtual TelemetryClient GetClient(ExecutionContext context)
        {
         
           // var key = FunctionsSettings.APPINSIGHTS_INSTRUMENTATIONKEY;
            string key = System.Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY", EnvironmentVariableTarget.Process);
            TelemetryClient client = new TelemetryClient()
            {
                InstrumentationKey = key
            };

            return client;
        }
    }

    public interface ITelemetryClientFactory
    {
        TelemetryClient GetClient(ExecutionContext context);
    }
}
