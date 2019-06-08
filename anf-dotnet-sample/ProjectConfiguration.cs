namespace AnfDotNetSample
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Identity.Client;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using AnfDotNetSample.Model;
    using System.Collections.Generic;
    using Microsoft.Azure.Management.NetApp.Models;

    /// <summary>
    /// Description of the configuration of an AzureAD public client application (desktop/mobile application). This should
    /// match the application registration done in the Azure portal
    /// Source solution: https://github.com/azure-samples/active-directory-dotnetcore-console-up-v2
    /// </summary>
    public class ProjectConfiguration
    {
        /// <summary>
        /// Authentication options
        /// </summary>
        public PublicClientApplicationOptions PublicClientApplicationOptions { get; set; }

        /// <summary>
        /// Gets or sets all necessary details to work with the resource
        /// </summary>
        public ResourceDetails ResourceDetails { get; set; }

        /// <summary>
        /// Gets or sets a list of accounts to be created
        /// </summary>
        public List<ModelNetAppAccount> Accounts { get; set; }

        /// <summary>
        /// Gets or sets the subcription Id where account(s) will be deployed
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the resource group where the ANF account(s) will be created
        /// </summary>
        public string ResourceGroup { get; set; }

        /// <summary>
        /// Reads the configuration from a json file
        /// </summary>
        /// <param name="path">Path to the configuration json file</param>
        /// <returns>SampleConfiguration as read from the json file</returns>
        public static ProjectConfiguration ReadFromJsonFile(string path)
        {
            // .NET configuration
            IConfigurationRoot dotnetConfig;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
                .AddJsonFile(path);

            dotnetConfig = builder.Build();

            // Initializing and reading the auth endpoint and other configuration information
            ProjectConfiguration config = new ProjectConfiguration()
            {
                PublicClientApplicationOptions = new PublicClientApplicationOptions(),
                Accounts = new List<ModelNetAppAccount>()
            };

            dotnetConfig.Bind("authentication", config.PublicClientApplicationOptions);
            dotnetConfig.Bind("resourceDetails", config.ResourceDetails);
            dotnetConfig.Bind("accounts", config.Accounts);

            config.SubscriptionId = dotnetConfig.GetValue<string>("general:subscriptionId");
            config.ResourceGroup = dotnetConfig.GetValue<string>("general:resourceGroup");
            
            return config;
        }
    }
}

