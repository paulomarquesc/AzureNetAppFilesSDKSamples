# anf-dotnet-sdk-sample
Repository that includes Azure NetApp File's SDK .NET code usage sample.

# Prerequisites

1. Azure Subscription
1. Subscription needs to be whitelisted during the gated GA period for Azure NetApp Files. For more information, please refer to [this](https://docs.microsoft.com/en-us/azure/azure-netapp-files/azure-netapp-files-register#waitlist) document.
1. Resource Group created
1. Virtual Network with a delegated subnet to Microsoft.Netapp/volumes resource. For more information, please refer to [Guidelines for Azure NetApp Files network planning](https://docs.microsoft.com/en-us/azure/azure-netapp-files/azure-netapp-files-network-topologies)
1. For this sample console app work we need to authenticate and in this sample application we are providing two ways, one that uses service principals (default) or using device flow authentication.
    1. For Service Principal based authentication
        1. Make sure you're logged on at the subscription where you want to be associated with the service principal by default:
            ```bash
            az account show
           ```
             If this is not the correct subscription, use             
             ```bash
            az account set -s <subscription name or id>  
            ```
        1. Create a service principal using Azure CLI
            ```bash
            az ad sp create-for-rbac --sdk-auth
            ```
        1. Copy the output content and paste it in a file called azureauth.json and secure it with file system permissions
        1. Set an environment variable pointing to the file path you just created, here is an example with Powershell and bash:
            Powershell 
            ```powershell
           [Environment]::SetEnvironmentVariable("AZURE_AUTH_LOCATION", "C:\sdksample\azureauth.json", "User")
            ```
            Bash
            ```bash
           export AZURE_AUTH_LOCATION=/sdksamples/azureauth.json
           ``` 
        1. By default the device flow based authentication code is commented within the sample code and sample appsettings.json file, make sure that they are still commented and that only service principal related code is not commented out at program.cs file.
        
        >Note: for more information on service principal authentication with dotnet, please refer to [Authenticate with the Azure Libraries for .NET](https://docs.microsoft.com/en-us/dotnet/azure/dotnet-sdk-azure-authenticate?view=azure-dotnet)
    1. For device flow authentication
     
        1. Make sure to comment service principal authentication code at program.cs file and that all code related to device authentication flow is not commented on it and that the authentication section of appsettings.json file is also not commented anymore. 
        1. Create a new application within your Azure Active Directory Tenant:
            ```bash
            az ad app create --display-name anf-sdk-samples --native-app --reply-url http://localhost --required-resource-accesses @manifest.json

            ```

            **manifest.json contents**
            ```json
            [{
                "resourceAppId": "797f4846-ba00-4fd7-ba43-dac1f8f63013",
                "resourceAccess": [
                    {
                        "id": "41094075-9dad-400e-a0bd-54e686782033",
                        "type": "Scope"
                    }
                ]
            }]
            ```
        1. Take a note of the ApplicationID value and change the value on `clientID' setting at anf-dotnet-sample\appsettings.json configuration file
        1. If the user signing-in does not have permissions to consent to these permissions described within the manifest.json file, you should ask your Azure Active Directory Global Admin to apply these permissions for you before executing the console application.
       
            To pre-approve these assigments:
                1. Within Azure Active Directory, click "App registrations"
                1. Start typing the application name in the search field until you see the application name in the list
                1. Click the application name and select "Api permissions"
                1. The list of required api permissions will be shown and to grant consent, just click the "Grant admin consent for `<Azure AD Name>`" button.
                ![aadconsent](./media/aadconsent.png)
        >Note: for more information on device flow authentication, please refer to [Azure Active Directory Device Flow Authentication full sample](https://aka.ms/msal-net-device-code-flow)

# How to run this test 


# References

[Azure Active Directory Device Flow Authentication full sample](https://aka.ms/msal-net-device-code-flow)
[Authenticate with the Azure Libraries for .NET](https://docs.microsoft.com/en-us/dotnet/azure/dotnet-sdk-azure-authenticate?view=azure-dotnet)
