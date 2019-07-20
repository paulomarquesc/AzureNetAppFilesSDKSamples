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
                RunAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }

        static private async Task RunAsync()
        {
            // Getting project configuration
            ProjectConfiguration config = GetConfiguration("appsettings.json");

            // Authenticating
            Console.ForegroundColor = ConsoleColor.Yellow;
            AuthenticationResult authenticationResult = await AuthenticateAsync(config.PublicClientApplicationOptions);
            Console.ResetColor();

            // Instantiating a new ANF management client
            Console.WriteLine("Instantiating a new Azure NetApp Files management client...");
            TokenCredentials credentials = new TokenCredentials(authenticationResult.AccessToken);
            AzureNetAppFilesManagementClient anfClient = new AzureNetAppFilesManagementClient(credentials) { SubscriptionId = config.SubscriptionId };
            Console.WriteLine($"\tApi Version: {anfClient.ApiVersion}");

            // Creating ANF resources (Account, Pool, Volumes)
            Creation.RunCreationSampleAsync(config, anfClient).GetAwaiter().GetResult();

            // Performing updates on Capacity Pools and Volumes
            Updates.RunUpdateOperationsSampleAsync(config, anfClient).GetAwaiter().GetResult();

            // Creating and restoring snapshots
            //Snapshots.RunSnapshotOperationsSampleAsync(config, anfClient).GetAwaiter().GetResult();

            // WARNING: destructive operations
            // Deletion operations (snapshots, volumes, capacity pools and accounts)
            Cleanup.RunCleanupTasksSampleAsync(config, anfClient).GetAwaiter().GetResult();
        }
    }
}
