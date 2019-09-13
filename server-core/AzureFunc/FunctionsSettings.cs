using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFunc
{
    public static class FunctionsSettings
    {
        public const string PartitionKey = "GABC";
        public const string TableName = "tblobject";
        public const string AzureWebJobsStorage = "AzureWebJobsStorage";
        public const string RouteBase = "objects";
     
        public const string RouteWithId = RouteBase + "/{id}";


        //Email Setting

        public const string PrimaryDomain = "smtp.gmail.com";
        public const int PrimaryPort = 587;
        public const string SecondayDomain = "smtp.live.com";
        public const string SecondaryPort = "587";
        public const string UsernameEmail = "user_email@gmail.com";
        public const string UsernamePassword = "UserPassword";
        public const string FromEmail = "fromEmail@gmail.com";
        public const string ToEmail = "toEmail@gmail.com";
        public const string CcEmail = "ccEmail";
    }
}
