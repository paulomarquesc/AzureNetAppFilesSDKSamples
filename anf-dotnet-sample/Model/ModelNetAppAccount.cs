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
    /// Instantiates a ModelNetAppAccount object
    /// </summary>
    public class ModelNetAppAccount
    {
        /// <summary>
        /// Gets or sets a list of capacity pools
        /// </summary>
        public List<ModelCapacityPool> CapacityPools { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }
    }
}
