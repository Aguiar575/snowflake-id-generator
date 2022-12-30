FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

WORKDIR /App
COPY ./ ./
RUN pwd && ls -la

RUN dotnet restore

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0

COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "SnowFlakeFactory.Api.dll"]

#docker build -t snowflake-factory -f Dockerfile . --progress=plain