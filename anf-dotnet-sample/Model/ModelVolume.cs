namespace AnfDotNetSample.Model
{
    using Microsoft.Azure.Management.NetApp.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Identity.Client;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
	
	/// <summary>
    /// Instantiates a ModelVolume object
    /// </summary>
    public class ModelVolume
    {
        /// <summary>
        /// Gets or sets exportPolicy
        /// </summary>
        public List<ModelExportPolicyRule> ExportPolicies { get; set; }

        /// <summary>
        /// Gets or sets usageThreshold
        /// </summary>
        /// <remarks>
        /// Maximum storage quota allowed for a file system in bytes. This is a soft quota used for alerting only.
        /// Minimum size is 100 GiB. Upper limit is 100TiB. Number must  be represented in bytes = 107370000000.
        /// </remarks>
        public long UsageThreshold { get; set; }

        /// <summary>
        /// Gets or sets creation Token or File Path
        /// </summary>
        /// <remarks>A unique file path for the volume. Used when creating mount targets</remarks>
        public string CreationToken { get; set; }
  
        /// <summary>
        /// Gets or sets volume type
        /// </summary>
        /// <remarks>Ällowed values are "NFSv3" or "SMB"</remarks>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets volume name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Azure Resource URI for a delegated subnet. Must have the delegation
        /// Microsoft.NetApp/volumes
        /// </summary>
        public string SubnetId { get; set; }
    }
}
