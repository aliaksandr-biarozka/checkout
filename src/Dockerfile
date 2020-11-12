FROM  mcr.microsoft.com/dotnet/sdk:5.0-alpine as builder
WORKDIR /source

COPY . ./
RUN dotnet restore ./API/API.csproj

RUN dotnet publish ./API/API.csproj -c Release  -o app

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine as runtime
WORKDIR /app
COPY --from=builder /source/app ./
ENTRYPOINT [ "dotnet", "API.dll" ]