using System;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.ApplicationInsights;
using System.Net.Http;
using Microsoft.WindowsAzure.Storage.Table;

using System.Net;
using System.Data.SqlClient;


using AzureFunc.Entities;
using System.Security.Claims;


namespace AzureFunc
{
    public static class CrudFunction
    {
        public static ITelemetryClientFactory telemetryFactory = new TelemetryClientFactory();

        #region Check Status



        [FunctionName("TestFunction")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]
            HttpRequest req,
            ILogger log)
        {

            var str = Environment.GetEnvironmentVariable("sqldb_connection");

            log.LogInformation("C# HTTP trigger function processed a request.");
            var obj = new ObjectItem
            {
                Description = "This is a desc",
                Title = "Test Title",
         
            };

            return new OkObjectResult(obj);   
      
        }

        [FunctionName("CheckContext")]
        public static IActionResult ReturnContext(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")]
            HttpRequest req,
            ILogger log, ClaimsPrincipal claimsPrinciple)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            IEnumerable<(string Type, string Value,string Issuer)> context = claimsPrinciple.Claims.Select(p => new System.ValueTuple<string, string, string>(p.Type, p.Value, p.Issuer));
         
            


            return new OkObjectResult(context);   
      
        }

        #endregion

        #region Azure Function CreateObject

        [FunctionName("CreateObject")]
        public static async Task<HttpResponseMessage> CreateObject(
             [HttpTrigger(AuthorizationLevel.Anonymous,
      
            "post")]HttpRequestMessage req,
             
             ILogger log, ClaimsPrincipal claimsPrinciple)
        {
            try
            {
                log.LogInformation("User:" + claimsPrinciple.Identity.Name);
                var json = await req.Content.ReadAsStringAsync();

                var objectInfo = JsonConvert.DeserializeObject<ObjectInfo>(json);

                objectInfo.createdBy = claimsPrinciple.Identity.Name;

                await CloudTableExtensions.AddOrUpdateToTable(objectInfo);


                return req.CreateResponse(HttpStatusCode.Created, objectInfo);
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
                log.LogError(e.StackTrace);
                
                return req.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
                throw e;
            }
        }


        #endregion

        #region Azure Function  GetObject

        [FunctionName("GetObject")]
        public static async Task<HttpResponseMessage> GetObject([HttpTrigger(AuthorizationLevel.Anonymous,
            Route = "GetObject/{id}")]HttpRequestMessage req, string id,
         
             ILogger log, ClaimsPrincipal claimsPrinciple, ExecutionContext context)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var item = await CloudTableExtensions.GetObjFromTable(id, claimsPrinciple.Identity.Name);

            stopWatch.Stop();

            var metrics = new Dictionary<string, double> {
                { "processingTime", stopWatch.Elapsed.TotalMilliseconds}
            };

            var props = new Dictionary<string, string> {
                { "object-id", id}
            };

            TelemetryClient telemetryClient = telemetryFactory.GetClient(context);
            telemetryClient.TrackEvent("get-object", metrics: metrics);

            return req.CreateResponse(HttpStatusCode.OK, item);
        }
        #endregion

        
        #region Azure Function UpdateObject

        [FunctionName("UpdateObject")]
        public static async Task<HttpResponseMessage> UpdateObject([HttpTrigger(AuthorizationLevel.Anonymous,
            "put", Route = "objects/{id}")]HttpRequestMessage req,
            string id,
            ILogger log, ExecutionContext context, ClaimsPrincipal claimsPrincipal)
        {
            Stopwatch stopWatch = new Stopwatch();

            var json = await req.Content.ReadAsStringAsync();
            var item = JsonConvert.DeserializeObject<ObjectInfo>(json);

            var oldItem = await CloudTableExtensions.GetObjFromTable(id, claimsPrincipal.Identity.Name);
            item.id = id; // ensure item id matches id passed in
            item.isComplete = oldItem.isComplete; // ensure we don't change isComplete
            item.createdBy = claimsPrincipal.Identity.Name;
            await CloudTableExtensions.AddOrUpdateToTable(item);

            stopWatch.Stop();

            var metrics = new Dictionary<string, double> {
                { "processingTime", stopWatch.Elapsed.TotalMilliseconds}
            };

            var props = new Dictionary<string, string> {
                { "object-id", id}
            };

            TelemetryClient telemetryClient = telemetryFactory.GetClient(context);
            telemetryClient.TrackEvent("update-object", properties: props, metrics: metrics);
            return req.CreateResponse(HttpStatusCode.OK, item);
        }

        #endregion

        #region Azure Function Delete Object

        [FunctionName("DeleteObject")]
        public static async Task<HttpResponseMessage> DeleteObject([HttpTrigger(AuthorizationLevel.Anonymous, "delete",
            Route = "objects/{id}")]HttpRequestMessage req, string id,
           
            ILogger log, ExecutionContext context, ClaimsPrincipal claimsPrinciple)
        {
            Stopwatch stopWatch = new Stopwatch();

            try
            {
                await CloudTableExtensions.DeleteObjFromTable(id, claimsPrinciple.Identity.Name);
            }
            catch (Exception ex)
            {

                throw ex;
            }
          

            stopWatch.Stop();

            var metrics = new Dictionary<string, double> {
                { "processingTime", stopWatch.Elapsed.TotalMilliseconds}
            };

            var props = new Dictionary<string, string> {
                { "object-id", id}
            };

            TelemetryClient telemetryClient = telemetryFactory.GetClient(context);
            telemetryClient.TrackEvent("delete-object", properties: props, metrics: metrics);

            return req.CreateResponse(HttpStatusCode.OK);
        }

        #endregion
        

        #region Azure Function GetAllObject From windows storage

        [FunctionName("GetAllObjects")]
        public static async Task<List<ObjectInfo>> GetAllObject([HttpTrigger(AuthorizationLevel.Anonymous,
            "get")]HttpRequest req,  ExecutionContext context, ILogger log, ClaimsPrincipal claimsPrinciple)
        {
            try
            {

            
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var item = await CloudTableExtensions.GetAllObject(claimsPrinciple.Identity.Name);

            stopWatch.Stop();

            var metrics = new Dictionary<string, double> {
                { "processingTime", stopWatch.Elapsed.TotalMilliseconds}
            };

          

            TelemetryClient telemetryClient = telemetryFactory.GetClient(context);
            telemetryClient.TrackEvent("get-all-objects", metrics: metrics);

            //return req.CreateResponse(HttpStatusCode.OK, item);
            return item.Select(i => i.MapFromTableEntity()).ToList(); ;

            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
        #endregion

    }


}
