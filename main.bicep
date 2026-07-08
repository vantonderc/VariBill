// infrastructure/main.bicep
param environmentName string = 'dev'
param location string = 'southafricanorth'
param sqlAdminPassword string

// Resource naming
var appServicePlanName = 'plan-${environmentName}'
var apiAppName = 'varibill-api-${environmentName}'
var mvcAppName = 'varibill-mvc-${environmentName}'
var blazorAppName = 'varibill-blazor-${environmentName}'
var identityAppName = 'varibill-identity-${environmentName}'
var sqlServerName = 'sql-${uniqueString(resourceGroup().id)}'
var sqlDbName = 'VariBillDB'

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'B1'
    tier: 'Basic'
  }
  kind: 'linux'
  properties: {
    reserved: true // Required for Linux
  }
}

// SQL Server
resource sqlServer 'Microsoft.Sql/servers@2022-05-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: 'varibilladmin'
    administratorLoginPassword: sqlAdminPassword
    version: '12.0'
  }
}

// SQL Database
resource sqlDatabase 'Microsoft.Sql/servers/databases@2022-05-01-preview' = {
  parent: sqlServer
  name: sqlDbName
  location: location
  sku: {
    name: 'S0'
    tier: 'Standard'
  }
}

// Allow Azure services to access SQL
resource firewallRule 'Microsoft.Sql/servers/firewallRules@2022-05-01-preview' = {
  parent: sqlServer
  name: 'AllowAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// API App Service
resource apiApp 'Microsoft.Web/sites@2022-09-01' = {
  name: apiAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNET|9.0'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'ConnectionStrings__DefaultConnection'
          value: 'Server=tcp:${sqlServerName}.database.windows.net,1433;Database=${sqlDbName};User ID=varibilladmin;Password=${sqlAdminPassword};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
        }
        {
          name: 'APISettings__IdentityServer'
          value: 'https://${identityAppName}.azurewebsites.net'
        }
      ]
    }
  }
}

// MVC App Service
resource mvcApp 'Microsoft.Web/sites@2022-09-01' = {
  name: mvcAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNET|9.0'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'APISettings__VariBillAPIAddress'
          value: 'https://${apiAppName}.azurewebsites.net'
        }
        {
          name: 'APISettings__IdentityServer'
          value: 'https://${identityAppName}.azurewebsites.net'
        }
        {
          name: 'APISettings__InteractiveClientId'
          value: 'VariBillWebApp.Interactive'
        }
        {
          name: 'APISettings__APISecret'
          value: 'G3cX6Dt9JhUmaZ8F'
        }
        {
          name: 'APISettings__APIScope'
          value: 'VariBillWebAPI'
        }
        {
          name: 'APISettings__APIClient'
          value: 'VariBillWebAPI.ClientCredentials'
        }
      ]
    }
  }
}

// Blazor App Service
resource blazorApp 'Microsoft.Web/sites@2022-09-01' = {
  name: blazorAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNET|9.0'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'APISettings__VariBillAPIAddress'
          value: 'https://${apiAppName}.azurewebsites.net'
        }
        {
          name: 'APISettings__IdentityServer'
          value: 'https://${identityAppName}.azurewebsites.net'
        }
        {
          name: 'APISettings__InteractiveClientId'
          value: 'VariBillWebApp.Interactive'
        }
        {
          name: 'APISettings__APISecret'
          value: 'G3cX6Dt9JhUmaZ8F'
        }
        {
          name: 'APISettings__APIScope'
          value: 'VariBillWebAPI'
        }
        {
          name: 'APISettings__APIClient'
          value: 'VariBillWebAPI.ClientCredentials'
        }
      ]
    }
  }
}

// IdentityServer App Service
resource identityApp 'Microsoft.Web/sites@2022-09-01' = {
  name: identityAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNET|9.0'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'ConnectionStrings__DefaultConnection'
          value: 'Server=tcp:${sqlServerName}.database.windows.net,1433;Database=${sqlDbName};User ID=varibilladmin;Password=${sqlAdminPassword};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
        }
      ]
    }
  }
}

output apiUrl string = apiApp.properties.defaultHostName
output mvcUrl string = mvcApp.properties.defaultHostName
output blazorUrl string = blazorApp.properties.defaultHostName
output identityUrl string = identityApp.properties.defaultHostName