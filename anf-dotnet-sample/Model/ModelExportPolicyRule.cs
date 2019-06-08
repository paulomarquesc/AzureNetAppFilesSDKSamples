namespace AnfDotNetSample.Model
{
    using Microsoft.Azure.Management.NetApp.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Identity.Client;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
	
	/// <summary>
    /// Instantiates a ModelExportPolicyRule object
    /// </summary>
    public class ModelExportPolicyRule
    {
        /// <summary>
        /// Gets or sets RuleIndex
        /// </summary>
        /// <remarks>This is initally non zero-based index, therefore must start with 1</remarks>
        public int RuleIndex { get; set; }

        /// <summary>
        /// Gets or sets AllowedClients
        /// </summary>
        public string AllowedClients { get; set; }

        /// <summary>
        /// Gets or sets CIFS
        /// </summary>
        public bool? Cifs { get; set; }

        /// <summary>
        /// Gets or sets Nfsv3
        /// </summary>
        public bool? Nfsv3 { get; set; }

        /// <summary>
        /// Gets or sets Nfsv4
        /// </summary>
        public bool? Nfsv4 { get; set; }

        /// <summary>
        /// Gets or sets UnixReadOnly
        /// </summary>
        public bool? UnixReadOnly { get; set; }

        /// <summary>
        /// Gets or sets UnixReadWrite
        /// </summary>
        public bool? UnixReadWrite { get; set; }
        
    }
}
