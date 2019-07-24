// Copyright (c) Microsoft and contributors.  All rights reserved.
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.

namespace Microsoft.Azure.Management.ANF.Samples
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.Management.ANF.Samples.Common;
    using Microsoft.Azure.Management.NetApp;
    using Microsoft.Azure.Management.ResourceManager.Fluent;
    using Microsoft.Identity.Client;
    using Microsoft.Rest;
    using static Microsoft.Azure.Management.ANF.Samples.Common.Utils;

    class program
    {
        /// <summary>
        /// Sample console application that execute CRUD management operations on Azure NetApp Files resource
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            DisplayConsoleAppHeader();

            try
            {
                //RunAsync().GetAwaiter().GetResult();
                Run();
            }
            catch (Exception ex)
            {
                WriteErrorMessage(ex.Message);
            }
        }

        static private void Run()
        {
            // Getting project configuration
            ProjectConfiguration config = GetConfiguration("appsettings.json");

            // Authenticating using service principals
            var credentials = SdkContext.AzureCredentialsFactory
                .FromFile(Environment.GetEnvironmentVariable("AZURE_AUTH_LOCATION"));

            // Authenticating using Device Login flow - uncomment following 4 lines for this type of authentication and
            // comment the lines related to service principal authentication
            // Console.ForegroundColor = ConsoleColor.Yellow;
            // AuthenticationResult authenticationResult = await AuthenticateAsync(config.PublicClientApplicationOptions);
            // TokenCredentials credentials = new TokenCredentials(authenticationResult.AccessToken);
            // Console.ResetColor();

            // Instantiating a new ANF management client
            Utils.WriteConsoleMessage("Instantiating a new Azure NetApp Files management client...");

            AzureNetAppFilesManagementClient anfClient = new AzureNetAppFilesManagementClient(credentials) { SubscriptionId = config.SubscriptionId };
            Utils.WriteConsoleMessage($"\tApi Version: {anfClient.ApiVersion}");

            // Creating ANF resources (Account, Pool, Volumes)
            Creation.RunCreationSampleAsync(config, anfClient).GetAwaiter().GetResult();

            // Performing updates on Capacity Pools and Volumes
            Updates.RunUpdateOperationsSampleAsync(config, anfClient).GetAwaiter().GetResult();

            // Creating and restoring snapshots
            Snapshots.RunSnapshotOperationsSampleAsync(config, anfClient).GetAwaiter().GetResult();

            // WARNING: destructive operations
            // Deletion operations (snapshots, volumes, capacity pools and accounts)

            // Waiting a few seconds before starting cleaning up process
            //System.Threading.Thread.Sleep(90);
            Cleanup.RunCleanupTasksSampleAsync(config, anfClient).GetAwaiter().GetResult();
        }
    }
}
