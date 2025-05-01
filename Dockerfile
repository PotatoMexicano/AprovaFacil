# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia os arquivos do projeto e restaura as dependências
COPY AprovaFacil.sln ./
COPY AprovaFacil.Application/AprovaFacil.Application.csproj AprovaFacil.Application/
COPY AprovaFacil.Client/AprovaFacil.Client.esproj AprovaFacil.Client/
COPY AprovaFacil.Domain/AprovaFacil.Domain.csproj AprovaFacil.Domain/
COPY AprovaFacil.Infra.Data/AprovaFacil.Infra.Data.csproj AprovaFacil.Infra.Data/
COPY AprovaFacil.Infra.IoC/AprovaFacil.Infra.IoC.csproj AprovaFacil.Infra.IoC/
COPY AprovaFacil.Server/AprovaFacil.Server.csproj AprovaFacil.Server/

RUN dotnet restore AprovaFacil.sln

# Copia o restante dos arquivos e publica a aplicação
COPY . ./
RUN dotnet publish AprovaFacil.Server/AprovaFacil.Server.csproj -c Release -o out

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Expõe a porta usada pela aplicação
EXPOSE 80

ENV ASPNETCORE_ENVIRONMENT=Production


# Comando para iniciar a aplicação
ENTRYPOINT ["dotnet", "AprovaFacil.Server.dll"]