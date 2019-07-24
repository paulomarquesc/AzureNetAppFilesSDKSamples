// Copyright (c) Microsoft and contributors.  All rights reserved.
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.

namespace Microsoft.Azure.Management.ANF.Samples
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Management.ANF.Samples.Common;
    using Microsoft.Azure.Management.ANF.Samples.Model;
    using Microsoft.Azure.Management.NetApp;
    using Microsoft.Azure.Management.NetApp.Models;
    using static Microsoft.Azure.Management.ANF.Samples.Common.Utils;
    using static Microsoft.Azure.Management.ANF.Samples.Common.Sdk.CommonSdk;
    using System.Collections;

    public static class Cleanup
    {
        /// <summary>
        /// Executes basic CRUD operations using Azure NetApp files SDK
        /// </summary>
        /// <returns></returns>
        public static async Task RunCleanupTasksSampleAsync(ProjectConfiguration config, AzureNetAppFilesManagementClient anfClient)
        {

            //
            // Cleaning up snapshots
            //

            Utils.WriteConsoleMessage("Cleaning up snapshots...");
            List<Task> snapshotTasks = new List<Task>();
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
                                IEnumerable<Snapshot> anfSnapshotList = null;

                                try
                                {
                                    anfSnapshotList = await anfClient.Snapshots.ListAsync(
                                       config.ResourceGroup,
                                       anfAcct.Name,
                                       pool.Name,
                                       volume.Name);
                                }
                                catch (Exception ex)
                                {
                                    if (ex.HResult == -2146233088)
                                    {
                                        Utils.WriteConsoleMessage($"No snapshots related to volume {volume.Name} found.");
                                    }
                                    else
                                    {
                                        Utils.WriteErrorMessage($"An error ocurred trying to list snapshots for volume {pool.Name}");
                                        throw;
                                    }
                                }

                                if (anfSnapshotList != null && anfSnapshotList.Count() > 0)
                                {
                                    // Snapshot Name property returns a relative path up to the name and to use this property
                                    // by the DeleteAsync parameter, the argument needs to be sanitized and just the 
                                    // actual name needs to be used.
                                    // Snapshot Name poperty example: "pmarques-anf01/pool01/pmarques-anf01-pool01-vol01/test-a"
                                    // "test-a" is the actual name that needs to be used instead. Below you will see a sample function that parses the name from 
                                    // the snapshot resource id

                                    snapshotTasks = anfSnapshotList.Select(
                                        async snapshot =>
                                        {
                                            await anfClient.Snapshots.DeleteAsync(config.ResourceGroup, anfAcct.Name, pool.Name, volume.Name, ResourceUriUtils.GetAnfSnapshot(snapshot.Id));
                                            Utils.WriteConsoleMessage($"\tDeleted snapshot {snapshot.Id}");
                                        }).ToList();
                                }
                                else
                                {
                                    Utils.WriteConsoleMessage($"No snapshots related to volume {volume.Name} found.");
                                }
                            }
                        }
                        else
                        {
                            Utils.WriteConsoleMessage($"\tNo volumes defined for Account: {anfAcct.Name}, Capacity Pool: {pool.Name}");
                        }
                    }
                }
            }

            try
            {
                await Task.WhenAll(snapshotTasks);
            }
            catch
            {
                if (OutputTaskErrorResults(snapshotTasks)) throw;
            }


            //
            // Cleaning up volumes
            //

            //
            // Cleaning up capacity pools
            //

            //
            // Cleaning up accounts
            //



        }
    }
}
