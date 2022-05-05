FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env

COPY . .
RUN dotnet publish ReviewFinishedAction.csproj -c Release -o out


# Label as GitHub action
LABEL com.github.actions.name="Review finished action"
LABEL com.github.actions.description="Review finished action"
LABEL com.github.actions.icon="sliders"
LABEL com.github.actions.color="purple"

# Relayer the .NET SDK, anew with the build output
FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY --from=build-env /out .
ENTRYPOINT [ "dotnet", "ReviewFinishedAction.dll" ]