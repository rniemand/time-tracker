#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
#RUN apt-get install --yes curl
#RUN curl --silent --location https://deb.nodesource.com/setup_10.x | bash -
#RUN apt-get install --yes nodejs

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
#RUN apt-get install --yes curl
#RUN curl --silent --location https://deb.nodesource.com/setup_10.x | bash -
#RUN apt-get install --yes nodejs

# Install NodeJs
RUN apt-get update && \
apt-get install -y wget && \
apt-get install -y gnupg2 && \
wget -qO- https://deb.nodesource.com/setup_12.x | bash - && \
apt-get install -y build-essential nodejs
# End Install

WORKDIR /src
COPY ["TimeTracker/TimeTracker.csproj", "TimeTracker/"]
COPY ["TimeTracker.Core/TimeTracker.Core.csproj", "TimeTracker.Core/"]
RUN dotnet restore "TimeTracker/TimeTracker.csproj"
COPY . .
WORKDIR "/src/TimeTracker"
RUN dotnet build "TimeTracker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TimeTracker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TimeTracker.dll"]