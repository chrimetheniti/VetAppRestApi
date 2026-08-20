# ============================================================
# Stage 1: build — SDK for compile
# This stage is discarded after build — it doesn't ship in the final image.
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy ONLY the .csproj first (better layer caching for restore).
COPY VetApp/*.csproj VetApp/
RUN dotnet restore VetApp/VetApp.csproj

# Copy the rest of the source and publish in Release mode.
COPY VetApp/ VetApp/
WORKDIR /src/VetApp
RUN dotnet publish -c Release -o /app

# ============================================================
# Stage 2: runtime — only ASP.NET runtime, no SDK, no source
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app .

# Documentation — actual host port mapping happens in docker-compose.
EXPOSE 8080

ENTRYPOINT ["dotnet", "VetApp.dll"]