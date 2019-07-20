// Copyright (c) Microsoft and contributors.  All rights reserved.
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.

namespace Microsoft.Azure.Management.ANF.Samples.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Management.ANF.Samples.Model;
    using Microsoft.Identity.Client;
    using Microsoft.Rest;
    using Microsoft.Rest.Azure;

    /// <summary>
    /// Contains public methods to get configuration settigns, to initiate authentication, output error results, etc.
    /// </summary>
    public static class Utils
    {

        /// <summary>
        /// Authentication scope, this is the minimum required to be able to manage resources
        /// </summary>
        private static string[] Scopes { get; set; } = new string[] { @"https://management.core.windows.net/.default" };

        /// <summary>
        /// Simple function to display this console app basic information
        /// </summary>
        public static void DisplayConsoleAppHeader()
        {
            Console.WriteLine("AzureNetAppFilesSDKSamples - Sample project that performs the following operations with Azure NetApp Files SDK:");
            Console.WriteLine("");
            Console.WriteLine("\t- Creates a new Azure NetApp Account");
            Console.WriteLine("\t- Creates a new Capacity Pool");
            Console.WriteLine("\t- Creates a new Volume");
            Console.WriteLine("\t- TBD");
            Console.WriteLine("");
        }

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

        /// <summary>
        /// Iterates over a Task<typeparamref name="T"/> for faulted tasks and outputs the error message and returns true when one or more faulted tasks are found
        /// </summary>
        /// <typeparam name="T">This can be NetAppAccount, CapacityPool, Volume</typeparam>
        /// <param name="tasks">List of tasks</param>
        /// <param name="level">Represents the identation of the error messages in spaces, in this sample we are using four spaces as convention</param>
        /// <returns></returns>
        public static bool OutputTaskErrorResults<T>(List<Task<T>> tasks)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            tasks.Where(task => task.IsFaulted).ToList()
                 .ForEach(task =>
                 {
                     Console.WriteLine($"\tTask Id: {task.Id}");
                     foreach (CloudException ce in task.Exception.InnerExceptions)
                     {
                         HttpRequestMessageWrapper request = ce.Request;
                         Console.WriteLine($"\t\tTask Exception Message: {ce.Message}");
                         Console.WriteLine($"\t\tTask Exception Request Content: {request.Content}");
                     }
                 });

            Console.ResetColor();

            return tasks.Where(task => task.IsFaulted).ToList().Count > 0;
        }

        public static bool OutputTaskErrorResults(List<Task> tasks)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            tasks.Where(task => task.IsFaulted).ToList()
                 .ForEach(task =>
                 {
                     Console.WriteLine($"\tTask Id: {task.Id}");
                     foreach (CloudException ce in task.Exception.InnerExceptions)
                     {
                         HttpRequestMessageWrapper request = ce.Request;
                         Console.WriteLine($"\t\tTask Exception Message: {ce.Message}");
                         Console.WriteLine($"\t\tTask Exception Request Content: {request.Content}");
                     }
                 });

            Console.ResetColor();

            return tasks.Where(task => task.IsFaulted).ToList().Count > 0;
        }

        /// <summary>
        /// Converts bytes into TiB
        /// </summary>
        /// <param name="size">Size in bytes</param>
        /// <returns>Returns (decimal) the value of bytes in TiB scale</returns>
        public static decimal GetBytesInTiB(long size)
        {
            return (decimal)size / 1024 / 1024 / 1024 / 1024;
        }

        /// <summary>
        /// Converts TiB into bytes
        /// </summary>
        /// <param name="size">Size in TiB</param>
        /// <returns>Returns (long) the value of TiB in bytes scale</returns>
        public static long GetTiBInBytes(decimal size)
        {
            return (long)size * 1024 * 1024 * 1024 * 1024;
        }

        /// <summary>
        /// Displays errors messages in red
        /// </summary>
        /// <param name="message">Message to be written in console</param>
        public static void WriteErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
