FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ExpenseTracker.sln", "./"]
COPY ["src/ExpenseTracker.API/ExpenseTracker.API.csproj", "src/ExpenseTracker.API/"]
COPY ["src/ExpenseTracker.Application/ExpenseTracker.Application.csproj", "src/ExpenseTracker.Application/"]
COPY ["src/ExpenseTracker.Core/ExpenseTracker.Core.csproj", "src/ExpenseTracker.Core/"]
COPY ["src/ExpenseTracker.Infrastructure/ExpenseTracker.Infrastructure.csproj", "src/ExpenseTracker.Infrastructure/"]
RUN dotnet restore
COPY . .
WORKDIR "/src/src/ExpenseTracker.API"
RUN dotnet build "ExpenseTracker.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExpenseTracker.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExpenseTracker.API.dll"]
