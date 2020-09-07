FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env

WORKDIR /build

RUN apt-get update && apt-get install -y curl
RUN curl -sL https://deb.nodesource.com/setup_14.x  | bash -
RUN apt-get install -y nodejs

COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime

WORKDIR /prod
COPY --from=build-env /build/out ./
ENTRYPOINT ["dotnet", "StatusWatchdog.dll"]
