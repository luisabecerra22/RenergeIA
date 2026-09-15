FROM mcr.microsoft.com/dotnet/aspnet:10.0

# Dependencias nativas para IKVM/MPXJ (lector de archivos Microsoft Project .mpp)
RUN apt-get update && apt-get install -y --no-install-recommends \
        libfontconfig1 libfreetype6 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
EXPOSE 8080

COPY publish/ .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "RenergeIA.Web.dll"]
