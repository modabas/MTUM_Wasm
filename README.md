# MTUM_Wasm
Multi tenant user authentication/authorization management infrastructure with;
1. AWS Cognito as identity provider with OAuth and Json web tokens,
2. Blazor wasm as frontend,
3. Web API as backend access point and Microsoft Orleans for scaleable stateful distributed service, virtual actor framework, data cache and scheduler,
4. Postgresql for additional tenant info and audit logs (with Dapper micro ORM) and as Microsoft Orleans management db.


## Setup
Run scripts in src/Server/MTUM_Wasm.Server.Infrastructure/Database/Postgres/Document/ in your Postgresql server to initialize Microsoft Orleans management db and MTUM_Wasm service db structure. They may be in same or different databases.

From AWS Console, setup a AWS Cognito user pool with following properties: login with email, no MFA, with a mutable custom attribute named "nac" with max langth 2048. Add following groups to user pool: "systemAdmin", "tenantAdmin", "tenantUser", "tenantViewer". Create an app client under user pool to be used from MTUM service for app integration.

Setup an IAM user with AmazonCognitoPowerUser policy. Its credentials will be used for the provider to access user pool.

Fill out src/Server/MTUM_Wasm.Server.Web/appsettings.Development.json (for development environment) with;
1. ConnectionStrings:ManagementDbConnStr -> Orleans management db connection string,
2. ServiceDbOptions:ConnectionString -> Service db connection string,
3. AwsCognito section -> AWS Cognito user pool and client application settings and also IAM user's access key id and secret.

Manually create first user within user pool from AWS Console and add to "systemAdmin" group. This user can now be used to login to MTUM application and perform actions on users including creating new system administators.

## How does it work?

### Authentication/Authorization
Users acquire JWT tokens from AWS Cognito on sign-in. These tokens are used by both frontend (as a best effort, because nothing on client side is really secure) to determine which pages are available to user and by backend Web Api. Each request to backend has tokens in request headers and these tokens are validated for each request to determine user is authenticated and attributes within tokens are evaluated to determine web method specific authorization policies are satisfied by calling user.

Tokens used in authentication and authorization are valid for an hour by default (can be modified from AWS Cognito setting in AWS Console). User also acquires a refreshToken on sign-in which is valid for a month by default. This refreshToken is being used by frontend to get new access and id tokens whenever they are close to expiration.

Selecting "Sign-out" from user interface clears tokens from browser cache, but sessions from other browsers remain intact.

Selecting "Sign-out Globally" from user interface clears tokens from browser cache and invalidates any refresh tokens granted until now. Other sessions already established at other browsers will remain intact until their access and id tokens expire (an hour max by default) but will not be refreshed from AWS Cognito because refresh tokens are invalidated. They will require to sign-in again.

### Why use resource owner password credentials grant over backend server?
When you have an interactive client (a user), it's better to implement authorization code grant flow if you can and communicate with authentication server (Aws Cognito in this case) to get tokens. However ROPC grant flow over your apis has its uses too, like to perform a [backend side one-at-a-time migration of existing users from a legacy authentication system](https://aws.amazon.com/blogs/mobile/migrating-users-to-amazon-cognito-user-pools/).
<br/>Note: This code does not include implemention of/call to a migration service during sign-in as explained in above document.

### User rights
Users in "systemAdmin" group can;
1. Create/modify tenants, users (for tenants or other system admins),
2. Assign nac policy (ip white/black lists) on a user or tenant basis,
3. View audit logs

Users in "tenantAdmin" group can;
1. Create/modify users (for their tenants), 
2. Assign nac policy (ip white/black lists) on a user or their tenant,
3. View audit logs for their tenant.

### Securing other services
Securing new apis in this project is done by simply decorating web methods with necessary policy attributes.

Any other independent service can be secured by utilizing authentication and authorization infrastructure by request token validation and policy based authorization similar to web api in this project. AWS Cognito specific token validation and attribute evaluation logic is implemented by following classes on server side:

```
services.AddSingleton<ITokenClaimResolver, AwsCognitoTokenClaimResolver>();
services.AddSingleton<IAwsCognitoConfigManager, AwsCognitoConfigManager>();
services.AddSingleton<IClaimsTransformation, AwsClaimsTransformation>();
```

## References
1. [Clean Architecture Template for Blazor WebAssembly Built with MudBlazor Components](https://github.com/blazorhero/CleanArchitecture): A huge inspiration for many parts of MTUM_Wasm, especially on the client side.
2. [HanBaoBao - Orleans sample application](https://github.com/ReubenBond/hanbaobao-web): Inspiration for co-hosting Orleans and Api Controllers in same process and request throttling implementation.
3. [ASP.NET Core Identity Provider for Amazon Cognito](https://github.com/aws/aws-aspnet-cognito-identity-provider): Wrapper methods around IAmazonCognitoIdentityProvider and CognitoUser of AWSSDK for AWS Cognito identity provider integration
4. [.NET Source Browser](https://source.dot.net/): Always a great reading material to understand what makes .NET tick and how. This project uses QueryHelper class source code in Blazor Wasm project, because QueryHelper is part of ASP.NET SDK and it can't be referenced from Blazor Wasm. Also severals parts of the application has code written with insights obtained from here.
