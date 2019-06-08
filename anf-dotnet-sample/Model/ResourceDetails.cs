namespace AnfDotNetSample.Model
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Identity.Client;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
	
	/// <summary>
    /// Creates instance of the resource and its details/dependencies
    /// </summary>
    public class ResourceDetails
    {
        /// <summary>
        /// Gets or sets the subscription Id to work with the resources
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the resource group name for the resource
        /// </summary>
        public string ResourceGroup { get; set; }

		/// <summary>
        /// Gets or sets the virtual network resource group, if null, application should assume ResourceGroup
        /// </summary>
		public string VirtualNetworkResourceGroup { get; set; }

		/// <summary>
        /// Gets or sets the virtual network name where ANF will be attached to
        /// </summary>
		public string VirtualNetworkName { get; set; }

		/// <summary>
        /// Gets or sets the subnet where the ANF NIC will be injected to
        /// </summary>
		public string SubnetName { get; set; }

		/// <summary>
        /// Gets or sets the ANF Account name
        /// </summary>
		public string AccountName { get; set; }

        /// <summary>
        /// Location where ANF account will be created
        /// </summary>
        public string Location { get; set; }
    }
}
