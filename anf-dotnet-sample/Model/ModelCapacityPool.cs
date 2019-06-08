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
    /// Creates instance of the ModelCapacityPool
    /// </summary>
    public class ModelCapacityPool
    {
        /// <summary>
        /// Gets or sets a list of volumes
        /// </summary>
        public List<ModelVolume> Volumes { get; set; }
 
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets size
        /// </summary>
        /// <remarks>Provisioned size of the pool (in bytes). Allowed values are in 4TiB chunks (value must be multiply of 4398046511104).</remarks>
        public long Size { get; set; }

        /// <summary>
        /// Gets or sets serviceLevel
        /// </summary>
        /// <remarks>The service level of the file system. Possible values include: 'Standard', 'Premium','Ultra'</remarks>
        public string ServiceLevel { get; set; }
    }
}
