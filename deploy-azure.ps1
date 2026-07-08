# scripts/deploy-azure.ps1
param(
    [string]$ResourceGroupName = "VariBillRG",
    [string]$Location = "southafricanorth",
    [string]$Environment = "dev",
    [string]$SqlAdminPassword
)

# Login to Azure
az login

# Create Resource Group
az group create --name $ResourceGroupName --location $Location

# Deploy Bicep template
az deployment group create `
    --resource-group $ResourceGroupName `
    --template-file ./infrastructure/main.bicep `
    --parameters environmentName=$Environment location=$Location sqlAdminPassword=$SqlAdminPassword

# Get the app names from the deployment
$apiName = "varibill-api-$Environment"
$mvcName = "varibill-mvc-$Environment"
$blazorName = "varibill-blazor-$Environment"
$identityName = "varibill-identity-$Environment"

Write-Host "Infrastructure deployed successfully!"
Write-Host "API URL: https://$apiName.azurewebsites.net"
Write-Host "MVC URL: https://$mvcName.azurewebsites.net"
Write-Host "Blazor URL: https://$blazorName.azurewebsites.net"
Write-Host "Identity URL: https://$identityName.azurewebsites.net"

# Publish and deploy each project
Write-Host "Building and deploying API..."
dotnet publish ../VariBillWebAPI/VariBillWebAPI.csproj -c Release -o ./publish/api
cd ./publish/api
Compress-Archive -Path * -DestinationPath ../api.zip -Force
az webapp deploy --src-path ../api.zip --resource-group $ResourceGroupName --name $apiName
cd ../..

Write-Host "Building and deploying MVC..."
dotnet publish ../VariBillWebApp.Mvc/VariBillWebApp.Mvc.csproj -c Release -o ./publish/mvc
cd ./publish/mvc
Compress-Archive -Path * -DestinationPath ../mvc.zip -Force
az webapp deploy --src-path ../mvc.zip --resource-group $ResourceGroupName --name $mvcName
cd ../..

Write-Host "Building and deploying Blazor..."
dotnet publish ../VariBillWebApp.Blazor/VariBillWebApp.Blazor.csproj -c Release -o ./publish/blazor
cd ./publish/blazor
Compress-Archive -Path * -DestinationPath ../blazor.zip -Force
az webapp deploy --src-path ../blazor.zip --resource-group $ResourceGroupName --name $blazorName
cd ../..

Write-Host "Building and deploying IdentityServer..."
dotnet publish ../Bongoe.Identity/Bongoe.Identity.csproj -c Release -o ./publish/identity
cd ./publish/identity
Compress-Archive -Path * -DestinationPath ../identity.zip -Force
az webapp deploy --src-path ../identity.zip --resource-group $ResourceGroupName --name $identityName
cd ../..

Write-Host "Deployment complete!"