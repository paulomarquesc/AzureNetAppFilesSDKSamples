namespace AnfDotNetSample
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AnfDotNetSample.Model;
    using Microsoft.Azure.Management.NetApp;
    using Microsoft.Azure.Management.NetApp.Models;
    using Microsoft.Identity.Client;
    using Microsoft.Rest;

    class program
    {
        private static int identation = 4;
        private static string level1 = new string(' ', identation);
        private static string level2 = new string(' ', identation * 2);

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

        /// <summary>
        /// Simple function to display this console app basic information
        /// </summary>
        static void DisplayConsoleAppHeader()
        {
            Console.WriteLine("Sample project that performs the following operations with Azure NetApp Files SDK:");
            Console.WriteLine("");
            Console.WriteLine($"{level1}- Creates a new Azure NetApp Account");
            Console.WriteLine($"{level1}- Creates a new Capacity Pool");
            Console.WriteLine($"{level1}- Creates a new Volume");
            Console.WriteLine($"{level1}- TBD");
            Console.WriteLine("");
        }
        
        /// <summary>
        /// Executes basic CRUD operations using Azure NetApp files SDK
        /// </summary>
        /// <returns></returns>
        private static async Task RunAsync()
        {
            // Getting project configuration
            ProjectConfiguration config = CoreHelper.GetConfiguration("appsettings.json");
                                    
            // Authenticating
            Console.ForegroundColor = ConsoleColor.Yellow;
            AuthenticationResult authenticationResult = await CoreHelper.AuthenticateAsync(config.PublicClientApplicationOptions);
            Console.ResetColor();

            // Instantiating a new ANF management client
            Console.WriteLine("Instantiating a new Azure NetApp Files management client...");
            TokenCredentials credentials = new TokenCredentials(authenticationResult.AccessToken);
            AzureNetAppFilesManagementClient anfClient = new AzureNetAppFilesManagementClient(credentials) {SubscriptionId = config.SubscriptionId};
            Console.WriteLine($"{level1}Api Version: {anfClient.ApiVersion}");
            
            // Creating ANF Accounts
            Console.WriteLine($"Creating Azure NetApp Files accounts ...");
            if (config.Accounts.Count == 0)
            {
                Console.WriteLine("No ANF accounts defined within appsettings.json file, exiting.");
                return;
            }
            else
            {
                List<Task<NetAppAccount>> accountTasks = new List<Task<NetAppAccount>>();
                foreach (ModelNetAppAccount anfAcct in config.Accounts)
                {
                    accountTasks.Add(Task.Run(
                        async () => await CreateOrRetrieveAccountAsync(config, anfClient, anfAcct)));
                }
                Task.WaitAll(accountTasks.ToArray());

                // Checking for errors - it returns true if an exception was found
                if (CoreHelper.OutputTaskResults<NetAppAccount>(accountTasks, level2))
                {
                    throw new Exception("One or more errors ocurred");
                }
            }

            // Creating Capacity Pools
            Console.WriteLine("Creating Capacity Pools...");
            List<Task<CapacityPool>> poolTasks = new List<Task<CapacityPool>>();
            foreach (ModelNetAppAccount anfAcct in config.Accounts)
            {
                if (anfAcct.CapacityPools != null)
                {
                    foreach (ModelCapacityPool pool in anfAcct.CapacityPools)
                    {
                        poolTasks.Add(Task.Run(
                            async () => await CreateOrRetrieveCapacityPoolAsync(anfClient, config.ResourceGroup, anfAcct, pool)));
                    }
                }
                else
                {
                    Console.WriteLine($"{level1}No capacity pool defined for account {anfAcct.Name}");
                }
            }
            Task.WaitAll(poolTasks.ToArray());

            // Checking for errors - it returns true if an exception was found
            if (CoreHelper.OutputTaskResults<CapacityPool>(poolTasks, level2))
            {
                throw new Exception("One or more errors ocurred");
            }

            // Creating Volumes
            Console.WriteLine("Creating Volumes...");
            List<Task<Volume>> volumeTasks = new List<Task<Volume>>();
            foreach (ModelNetAppAccount anfAcct in config.Accounts)
            {
                if (anfAcct.CapacityPools != null)
                {
                    foreach (ModelCapacityPool pool in anfAcct.CapacityPools)
                    {
                        if (pool.Volumes != null)
                        {

                            foreach (ModelVolume volume in pool.Volumes)
                            {
                                volumeTasks.Add(Task.Run(
                                    async () => await CreateOrRetrieveVolumeAsync(anfClient, config.ResourceGroup, anfAcct, pool, volume)));
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{level1}No volumes defined for Account: {anfAcct.Name}, Capacity Pool: {pool.Name}");
                        }
                    }
                }
            }
            Task.WaitAll(volumeTasks.ToArray());

            // Checking for errors - it returns true if an exception was found
            if (CoreHelper.OutputTaskResults<Volume>(volumeTasks, level2))
            {
                throw new Exception("One or more errors ocurred");
            }
        }

        /// <summary>
        /// Creates or retrieves an Azure NetApp Files Account
        /// </summary>
        /// <param name="config">Project Configuration file which contains the resource group needed</param>
        /// <param name="client">Azure NetApp Files Management Client</param>
        /// <param name="account">ModelNetAppAccount object that contains the data configured in the appsettings.json file for the ANF account</param>
        /// <returns>NetAppCount object</returns>
        private static async Task<NetAppAccount> CreateOrRetrieveAccountAsync(ProjectConfiguration config, AzureNetAppFilesManagementClient client, ModelNetAppAccount account)
        {
            // Creating the ANF Account
            NetAppAccount anfAccount;
            try
            {
                // Checking if resource already exists
                anfAccount = await client.Accounts.GetAsync(config.ResourceGroup, account.Name);
                Console.WriteLine($"{level1}Account already exists, resource id: {anfAccount.Id}");
            }
            catch (Exception ex)
            {
                // If account does not exist, create one
                if (ex.HResult == -2146233088)
                {
                    anfAccount = await CreateOrUpdateAnfAccountAsync(config, client, account);
                    Console.WriteLine($"{level1}Account successfully created, resource id: {anfAccount.Id}");
                }
                else
                {
                    throw;
                }
            }

            return anfAccount;
        }

        /// <summary>
        /// Creates or retrieves a Capacity Pool
        /// </summary>
        /// <param name="client">Azure NetApp Files Management Client</param>
        /// <param name="resourceGroup">Resource Group name where the capacity pool will be created</param>
        /// <param name="account">ModelNetAppAccount object that contains the data configured in the appsettings.json file for the ANF account</param>
        /// <param name="pool">ModelCapacityPool object that describes the capacity pool to be created, this information comes from appsettings.json</param>
        /// <returns>CapacityPool object</returns>
        private static async Task<CapacityPool> CreateOrRetrieveCapacityPoolAsync(AzureNetAppFilesManagementClient client, string resourceGroup, ModelNetAppAccount account, ModelCapacityPool pool)
        {
            // Creating the ANF Account
            CapacityPool anfCapacityPool;
            try
            {
                // Checking if resource already exists
                anfCapacityPool = await client.Pools.GetAsync(resourceGroup, account.Name, pool.Name);

                Console.WriteLine($"{level1}Capacity Pool already exists, resource id: {anfCapacityPool.Id}");
            }
            catch (Exception ex)
            {
                // If account does not exist, create one
                if (ex.HResult == -2146233088)
                {
                    anfCapacityPool = await CreateOrUpdateCapacityPoolAsync(client, resourceGroup, account, pool);
                    Console.WriteLine($"{level1}Capacity Pool successfully created, resource id: {anfCapacityPool.Id}");
                }
                else
                {
                    throw;
                }
            }

            return anfCapacityPool;
        }
        
        /// <summary>
        /// Creates or retrieves volume
        /// </summary>
        /// <param name="config">Project Configuration file which contains the resource group needed</param>
        /// <param name="client">Azure NetApp Files Management Client</param>
        /// <param name="account">ModelNetAppAccount object that contains the data configured in the appsettings.json file for the ANF account</param>
        /// <returns>NetAppCount object</returns>
        private static async Task<Volume> CreateOrRetrieveVolumeAsync(AzureNetAppFilesManagementClient client, string resourceGroup, ModelNetAppAccount account, ModelCapacityPool pool, ModelVolume volume)
        {
            // Creating or retrieving a volume
            Volume anfVolume;
            try
            {
                // Checking if resource already exists
                anfVolume = await client.Volumes.GetAsync(resourceGroup, account.Name, pool.Name, volume.Name);
                Console.WriteLine($"{level1}Volume already exists, resource id: {anfVolume.Id}");
            }
            catch (Exception ex)
            {
                // If volume does not exist, create one
                if (ex.HResult == -2146233088)
                {
                    anfVolume = await CreateOrUpdateVolumeAsync(client, resourceGroup, account, pool, volume);
                    Console.WriteLine($"{level1}Volume Pool successfully created, resource id: {anfVolume.Id}");
                }
                else
                {
                    throw;
                }
            }

            return anfVolume;
        }

        /// <summary>
        /// Creates or updates a Azure NetApp Files Account
        /// </summary>
        /// <param name="client">Azure NetApp Files Management Client</param>
        /// <param name="account">Account object generated from information contained at the appsettings.json file at Accounts section</param>
        /// <returns>NetAppAccount</returns>
        private static async Task<NetAppAccount> CreateOrUpdateAnfAccountAsync(ProjectConfiguration config, AzureNetAppFilesManagementClient client, ModelNetAppAccount account)
        {
            // Setting up NetApp Files account object
            NetAppAccount anfAccountBody = new NetAppAccount(account.Location, null, account.Name);

            // Requesting account to be created
            return await client.Accounts.CreateOrUpdateAsync(anfAccountBody, config.ResourceGroup, account.Name);
        }

        /// <summary>
        /// Creates or updates a Azure NetApp Files Account with Active Directory information for SMB
        /// </summary>
        /// <param name="client">Azure NetApp Files Management Client</param>
        /// <param name="account">Account object generated from information contained at the appsettings.json file at Accounts section</param>
        /// <param name="activeDirectoryInfoList">Active Directory object list</param>
        /// <returns>NetAppAccount object</returns>
        private static async Task<NetAppAccount> CreateOrUpdateAnfAccountAsync(ProjectConfiguration config, AzureNetAppFilesManagementClient client, ModelNetAppAccount account, ActiveDirectory[] activeDirectories)
        {
            // Setting up NetApp Files account object and Active Directory Information
            NetAppAccount anfAccount = new NetAppAccount(account.Location.ToLower(), null, account.Name, null, null, null, activeDirectories);

            // Requesting account to be created
            return await client.Accounts.CreateOrUpdateAsync(anfAccount, config.ResourceGroup, account.Name);
        }

        /// <summary>
        /// Creates or updates a capacity pool
        /// </summary>
        /// <param name="client">Azure NetApp Files Management Client</param>
        /// <param name="resourceGroup">Resource Group name where capacity pool will be created</param>
        /// <param name="account">Account object generated from information contained at the appsettings.json file at Accounts section</param>
        /// <param name="pool">ModelCapacityPool object that describes the capacity pool to be created, this information comes from appsettings.json</param>
        /// <returns>CapacityPool object</returns>
        private static async Task<CapacityPool> CreateOrUpdateCapacityPoolAsync(AzureNetAppFilesManagementClient client, string resourceGroup, ModelNetAppAccount account, ModelCapacityPool pool)
        {
            CapacityPool capacityPoolBody = new CapacityPool()
            {
                Location = account.Location.ToLower(),
                ServiceLevel = pool.ServiceLevel,
                Size = pool.Size
            };

            return await client.Pools.CreateOrUpdateAsync(capacityPoolBody, resourceGroup, account.Name, pool.Name);
        }

        /// <summary>
        /// Creates or updates a capacity pool
        /// </summary>
        /// <param name="client">Azure NetApp Files Management Client</param>
        /// <param name="resourceGroup">Resource Group name where volume will be created</param>
        /// <param name="account">Account object generated from information contained at the appsettings.json file at Accounts section</param>
        /// <param name="pool">ModelCapacityPool object that describes the capacity pool to be created, this information comes from appsettings.json</param>
        /// <param name="volume">ModelVolume object that represents the volume to be created that is defined in appsettings.json</param>
        /// <returns>Volume object</returns>
        private static async Task<Volume> CreateOrUpdateVolumeAsync(AzureNetAppFilesManagementClient client, string resourceGroup, ModelNetAppAccount account, ModelCapacityPool pool, ModelVolume volume)
        {
            List<ExportPolicyRule> ruleList = new List<ExportPolicyRule>();
            foreach (ModelExportPolicyRule rule in volume.ExportPolicies)
            {
                ruleList.Add(new ExportPolicyRule()
                {
                    AllowedClients = rule.AllowedClients,
                    Cifs = rule.Cifs,
                    Nfsv3 = rule.Nfsv3,
                    Nfsv4 = rule.Nfsv4,
                    RuleIndex = rule.RuleIndex,
                    UnixReadOnly = rule.UnixReadOnly,
                    UnixReadWrite = rule.UnixReadWrite
                });
            }

            VolumePropertiesExportPolicy exportPolicies = new VolumePropertiesExportPolicy() { Rules = ruleList };

            Volume volumeBody = new Volume()
            {
                ExportPolicy = exportPolicies,
                Location = account.Location.ToLower(),
                ServiceLevel = pool.ServiceLevel,
                CreationToken = volume.CreationToken,
                SubnetId = volume.SubnetId,
                UsageThreshold = volume.UsageThreshold
            };

            return await client.Volumes.CreateOrUpdateAsync(volumeBody, resourceGroup, account.Name, pool.Name, volume.Name);
        }
    }
}
