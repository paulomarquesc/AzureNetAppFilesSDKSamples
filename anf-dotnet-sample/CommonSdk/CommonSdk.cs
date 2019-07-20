// Copyright (c) Microsoft and contributors.  All rights reserved.
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.

namespace Microsoft.Azure.Management.ANF.Samples.Common.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.Azure.Management.ANF.Samples.Model;
    using Microsoft.Azure.Management.NetApp;
    using Microsoft.Azure.Management.NetApp.Models;

    /// <summary>
    /// Contains public methods for SDK related operations
    /// </summary>
    public static class CommonSdk
    {

        /// <summary>
        /// Creates or updates a Azure NetApp Files Account
        /// </summary>
        /// <param name="client">Azure NetApp Files Management Client</param>
        /// <param name="account">Account object generated from information contained at the appsettings.json file at Accounts section</param>
        /// <returns>NetAppAccount</returns>
        public static async Task<NetAppAccount> CreateOrUpdateAnfAccountAsync(ProjectConfiguration config, AzureNetAppFilesManagementClient client, ModelNetAppAccount account)
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
        public static async Task<NetAppAccount> CreateOrUpdateAnfAccountAsync(ProjectConfiguration config, AzureNetAppFilesManagementClient client, ModelNetAppAccount account, ActiveDirectory[] activeDirectories)
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
        public static async Task<CapacityPool> CreateOrUpdateCapacityPoolAsync(AzureNetAppFilesManagementClient client, string resourceGroup, ModelNetAppAccount account, ModelCapacityPool pool)
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
        /// Creates or updates a volume. In this process, notice that we need to create two mandatory objects, one as the export rule list and the voluome body itself before
        /// we request the volume creation.
        /// </summary>
        /// <param name="client">Azure NetApp Files Management Client</param>
        /// <param name="resourceGroup">Resource Group name where volume will be created</param>
        /// <param name="account">Account object generated from information contained at the appsettings.json file at Accounts section</param>
        /// <param name="pool">ModelCapacityPool object that describes the capacity pool to be created, this information comes from appsettings.json</param>
        /// <param name="volume">ModelVolume object that represents the volume to be created that is defined in appsettings.json</param>
        /// <returns>Volume object</returns>
        public static async Task<Volume> CreateOrUpdateVolumeAsync(AzureNetAppFilesManagementClient client, string resourceGroup, ModelNetAppAccount account, ModelCapacityPool pool, ModelVolume volume)
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

        /// <summary>
        /// Deletes a snapshot that is associated to a volume.
        /// </summary>
        /// <param name="client">Azure NetApp Files Management Client</param>
        /// <param name="resourceGroup">Resource Group name where volume will be created</param>
        /// <param name="account">Account object generated from information contained at the appsettings.json file at Accounts section</param>
        /// <param name="pool">ModelCapacityPool object that describes the capacity pool to be created, this information comes from appsettings.json</param>
        /// <param name="volume">ModelVolume object that represents the volume to be created that is defined in appsettings.json</param>
        /// <param name="snapshotName">Snapshot name</param>
        /// <returns></returns>
        public static async Task DeleteSnapshot(AzureNetAppFilesManagementClient client, string resourceGroup, ModelNetAppAccount account, ModelCapacityPool pool, ModelVolume volume, string snapshotName)
        {
            try
            {
                await client.Snapshots.DeleteAsync(resourceGroup, account.Name, pool.Name, volume.Name, snapshotName);
                Console.WriteLine($"\tSnapshot {snapshotName} successfuly deleted");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
