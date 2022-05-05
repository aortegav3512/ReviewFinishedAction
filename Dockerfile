FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ReviewFinishedAction/. .
RUN dotnet restore "ReviewFinishedAction.csproj"
RUN dotnet build "ReviewFinishedAction.csproj" -c Release
RUN dotnet publish "ReviewFinishedAction.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["ls", "-la"]