# MTUM_Wasm
Multi tenant user management infrastructure with;
1. AWS Cognito as identity provider with OAuth and Json web tokens,
2. Blazor wasm as frontend,
3. Microsoft Orleans for scaleable stateful distributed service, virtual actor framework, data cache and scheduler,
4. Postgresql for additional tenant info and audit logs and as Microsoft Orleans management db.


## Setup
Run scripts in src/Server/MTUM_Wasm.Server.Infrastructure/Database/Postgres/Document/ in your Postgresql server to initialize Microsoft Orleans management db and MTUM_Wasm service db structure. They may be in same or different databases.

Setup a AWS Cognito user pool with following properties: login with email, no MFA, with a mutable custom attribute named "nac" with max langth 2048. Add following groups to user pool: "systemAdmin", "tenantAdmin", "tenantUser", "tenantViewer". Create an app client under user pool to be used from MTUM service for app integration.

Fill out src/Server/MTUM_Wasm.Server.Web/appsettings.Development.json (for development environment) with Orleans management db connection string, Service db connection string and AWS Cognito user pool and client application settings

## References
1. [Clean Architecture Template for Blazor WebAssembly Built with MudBlazor Components](https://github.com/blazorhero/CleanArchitecture): A huge inspiration for many parts of MTUM_Wasm, especially on the client side.
2. [HanBaoBao - Orleans sample application](https://github.com/ReubenBond/hanbaobao-web): Inspiration for co-hosting Orleans and Api Controllers in same process and request throttling implementation.
3. [ASP.NET Core Identity Provider for Amazon Cognito](https://github.com/aws/aws-aspnet-cognito-identity-provider): Wrapper methods around IAmazonCognitoIdentityProvider and CognitoUser of AWSSDK for AWS Cognito identity provider integration
4. [.NET Source Browser](https://source.dot.net/): Always a great reading material to understand what makes .NET tick and how. This project uses QueryHelper class source code in Blazor Wasm project, because QueryHelper is part of ASP.NET SDK and it can't be referenced from Blazor Wasm.