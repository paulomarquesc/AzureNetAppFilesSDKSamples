﻿// Copyright (c) Microsoft and contributors.  All rights reserved.
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
    
    public static class Snapshots
    {
        /// <summary>
        /// Executes Snapshot related operations
        /// </summary>
        /// <returns></returns>
        public static async Task RunSnapshotOperationsSampleAsync(ProjectConfiguration config, AzureNetAppFilesManagementClient anfClient)
        {
            //
            // Creating snapshot from first volume of the first capacity pool
            //

            Console.WriteLine("Performing snapshot operations");

            Console.WriteLine("\tCreating snapshot...");

            string snapshotName = $"Snapshot-{Guid.NewGuid()}";

            Snapshot snapshotBody = new Snapshot()
            {
                Location = config.Accounts[0].Location
            };

            Snapshot snapshot = null;
            try
            {
                snapshot = await anfClient.Snapshots.CreateAsync(
                    snapshotBody,
                    config.ResourceGroup,
                    config.Accounts[0].Name,
                    config.Accounts[0].CapacityPools[0].Name,
                    config.Accounts[0].CapacityPools[0].Volumes[0].Name,
                    snapshotName);

                Console.WriteLine($"Snapshot successfully created. Snapshot resource id: {snapshot.Id}");
            }
            catch (Exception ex)
            {
                Utils.WriteErrorMessage($"An error occured while creating a snapshot of volume {config.Accounts[0].CapacityPools[0].Volumes[0].Name}.\nError message: {ex.Message}");
                throw;
            }

            ////
            //// Creating a volume from snapshot
            ////

            //Console.WriteLine("\tCreating new volume from snapshot...");

            //string newVolumeName = $"Vol-{ResourceUriUtils.GetAnfSnapshot(snapshot.Id)}";

            //Volume snapshotVolume = null;
            //try
            //{
            //    snapshotVolume = await anfClient.Volumes.GetAsync(
            //        ResourceUriUtils.GetResourceGroup(snapshot.Id),
            //        ResourceUriUtils.GetAnfAccount(snapshot.Id),
            //        ResourceUriUtils.GetAnfCapacityPool(snapshot.Id),
            //        ResourceUriUtils.GetAnfVolume(snapshot.Id));
            //}
            //catch (Exception ex)
            //{
            //    Utils.WriteErrorMessage($"An error occured trying to obtain information about volume ({ResourceUriUtils.GetAnfVolume(snapshot.Id)}) from snapshot {snapshot.Id}.\nError message: {ex.Message}");
            //    throw;
            //}

            //Volume newVolumeFromSnapshot = null;
            //try
            //{
            //    Volume volumeFromSnapshotBody = new Volume()
            //    {
            //        SnapshotId = snapshot.Id,
            //        ExportPolicy = snapshotVolume.ExportPolicy,
            //        Location = snapshotVolume.Location,
            //        ProtocolTypes = snapshotVolume.ProtocolTypes,
            //        ServiceLevel = snapshotVolume.ServiceLevel,
            //        UsageThreshold = snapshotVolume.UsageThreshold,
            //        SubnetId = snapshotVolume.SubnetId,
            //        CreationToken = newVolumeName
            //    };

            //    newVolumeFromSnapshot = await anfClient.Volumes.CreateOrUpdateAsync(
            //        volumeFromSnapshotBody,
            //        config.ResourceGroup,
            //        config.Accounts[0].Name,
            //        config.Accounts[0].CapacityPools[0].Name,
            //        newVolumeName);

            //    Console.WriteLine($"Volume successfully created from snapshot. Volume resource id: {newVolumeFromSnapshot.Id}");
            //}
            //catch (Exception ex)
            //{
            //    Utils.WriteErrorMessage($"An error occured while creating a volume ({newVolumeName}) from snapshot {snapshot.Id}.\nError message: {ex.Message}");
            //    throw;
            //}
        }
    }
}