# rsp-validate-irasid-function

# Introduction

This project exposes an API to accept requests with IRASID. It then looks up HARPProjectData to find the corresponding project and validate the IRASID. If the IRASID is valid, it returns a success response. If the IRASID is invalid, it returns an error response.

# Authentication
This API uses Azure Active Directory (AAD) for authentication. Clients must obtain an access token from AAD and include it in the Authorization header of their requests.

## Client Side
Ideally use System Assigned Identity and configure the Principal ID on the Allowed Principals list in the HARPProjectData Authentication settings. This allows the function to authenticate securely without needing to manage credentials.

## Validate IRASID Function Authentication Configuration
1. Authentication enabled (through Bicep code) will automatically create an App Registration on Entra ID with the same name as the function app. This App Registration will be used for authentication.
2. App Service Authentication is enabled with UnAuthenticated requests set to return HTTP 401 Unauthorized. This ensures that only authenticated requests can access the function.
3. An environment variable "OVERRIDE_USE_MI_FIC_ASSERTION_CLIENTID" is set to Pricipal ID of System Identity, which allows the function to use its System Assigned Identity to validate tokens with App Registration in Entra ID. This allows us to not have to manage credentials (client secrets) for the function app and leverage Azure's managed identity capabilities for secure authentication.

Documentation Link: https://learn.microsoft.com/en-us/azure/app-service/configure-authentication-provider-aad?tabs=workforce-configuration#use-a-managed-identity-instead-of-a-secret-preview

# API Endpoint
POST /validate-irasid

Request Body:
```json
{
  "irasid": "string"
}
```
# Deployment
This function can be deployed to Azure Functions. Ensure that the necessary environment variables and configurations are set up for the function to access HARPProjectData and perform authentication with AAD.

# Conclusion
This API provides a simple interface for validating IRASIDs against HARPProjectData. It ensures that only valid IRASIDs are accepted, enhancing the integrity of the data and the overall system.