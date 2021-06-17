FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build

# Install NodeJs
RUN apt-get update && \
apt-get install -y wget && \
apt-get install -y gnupg2 && \
wget -qO- https://deb.nodesource.com/setup_16.x | bash - && \
apt-get install -y build-essential nodejs
# End Install

WORKDIR /src
COPY ["/src/TimeTracker/TimeTracker.csproj", "/rn-build/TimeTracker/"]
COPY ["/src/TimeTracker.Core/TimeTracker.Core.csproj", "/rn-build/TimeTracker.Core/"]
RUN dotnet restore "/rn-build/TimeTracker/TimeTracker.csproj"

COPY /src/TimeTracker/ /rn-build/TimeTracker/
COPY /src/TimeTracker.Core/ /rn-build/TimeTracker.Core/
WORKDIR /rn-build/TimeTracker/
RUN dotnet build "TimeTracker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TimeTracker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TimeTracker.dll"]
