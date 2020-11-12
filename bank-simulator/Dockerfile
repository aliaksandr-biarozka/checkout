FROM  mcr.microsoft.com/dotnet/sdk:5.0-alpine as builder
WORKDIR /source

COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release  -o app

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine as runtime
WORKDIR /app
COPY --from=builder /source/app .
ENTRYPOINT [ "dotnet", "BankSimulator.dll" ]