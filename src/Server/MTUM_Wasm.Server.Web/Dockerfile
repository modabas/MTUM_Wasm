#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Server/MTUM_Wasm.Server.Web/MTUM_Wasm.Server.Web.csproj", "src/Server/MTUM_Wasm.Server.Web/"]
COPY ["src/Server/MTUM_Wasm.Server.Infrastructure/MTUM_Wasm.Server.Infrastructure.csproj", "src/Server/MTUM_Wasm.Server.Infrastructure/"]
COPY ["src/Shared/MTUM_Wasm.Shared.Infrastructure/MTUM_Wasm.Shared.Infrastructure.csproj", "src/Shared/MTUM_Wasm.Shared.Infrastructure/"]
COPY ["src/Shared/MTUM_Wasm.Shared.Core/MTUM_Wasm.Shared.Core.csproj", "src/Shared/MTUM_Wasm.Shared.Core/"]
COPY ["src/Server/MTUM_Wasm.Server.Core/MTUM_Wasm.Server.Core.csproj", "src/Server/MTUM_Wasm.Server.Core/"]
COPY ["src/Client/MTUM_Wasm.Client.Web/MTUM_Wasm.Client.Web.csproj", "src/Client/MTUM_Wasm.Client.Web/"]
COPY ["src/Client/MTUM_Wasm.Client.Infrastructure/MTUM_Wasm.Client.Infrastructure.csproj", "src/Client/MTUM_Wasm.Client.Infrastructure/"]
COPY ["src/Client/MTUM_Wasm.Client.Core/MTUM_Wasm.Client.Core.csproj", "src/Client/MTUM_Wasm.Client.Core/"]
RUN dotnet restore "src/Server/MTUM_Wasm.Server.Web/MTUM_Wasm.Server.Web.csproj"
COPY . .
WORKDIR "/src/src/Server/MTUM_Wasm.Server.Web"
RUN dotnet build "MTUM_Wasm.Server.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MTUM_Wasm.Server.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MTUM_Wasm.Server.Web.dll"]