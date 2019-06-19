
namespace AnfDotNetSample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Identity.Client;
    using Microsoft.Rest;
    using Microsoft.Rest.Azure;

    class CoreHelper
    {

        /// <summary>
        /// Authentication scope, this is the minimum required to be able to manage resources
        /// </summary>
        private static string[] Scopes { get; set; } = new string[] { @"https://management.core.windows.net/.default" };

        /// <summary>
        /// Function to create the configuration object, used for authentication and ANF resource information
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static ProjectConfiguration GetConfiguration(string filename)
        {
            return ProjectConfiguration.ReadFromJsonFile(filename);
        }

        /// <summary>
        /// Function to authenticate against Azure AD using MSAL 3.0 
        /// </summary>
        /// <param name="appConfig">Application configuration required for the authentication process</param>
        /// <returns>AuthenticationResult object</returns>
        public static async Task<AuthenticationResult> AuthenticateAsync(PublicClientApplicationOptions appConfig)
        {
            var app = PublicClientApplicationBuilder.CreateWithApplicationOptions(appConfig)
                                                    .Build();

            PublicAppUsingDeviceCodeFlow tokenAcquisitionHelper = new PublicAppUsingDeviceCodeFlow(app);

            return (await tokenAcquisitionHelper.AcquireATokenFromCacheOrDeviceCodeFlowAsync(Scopes));
        }

        public static bool OutputTaskResults<T>(List<Task<T>> tasks, string level)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            tasks.Where(task => task.IsFaulted).ToList()
                 .ForEach(task =>
                 {
                     Console.WriteLine($"{level}Task Id: {task.Id}");
                     foreach (CloudException ce in task.Exception.InnerExceptions)
                     {
                         HttpRequestMessageWrapper request = ce.Request;
                         Console.WriteLine($"{level}{level}Task Exception Message: {ce.Message}");
                         Console.WriteLine($"{level}{level}Task Exception Request Content: {request.Content}");
                     }
                 });

            Console.ResetColor();
            
            return tasks.Where(task => task.IsFaulted).ToList().Count > 0;
        }
    }
}
