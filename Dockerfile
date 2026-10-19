# --- STAGE 1: BUILD & SELF-CONTAINED PUBLISH (.NET 10) ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["RunxRun.csproj", "./"]
RUN dotnet restore "RunxRun.csproj"

# Copy the rest of the source code and static files (including wwwroot)
COPY . .

# Compile and publish as a self-contained application for Linux x64
RUN dotnet publish "RunxRun.csproj" -c Release -r linux-x64 --self-contained true -o /app/publish

# --- STAGE 2: FINAL RUNTIME IMAGE ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Expose port 8080 for Render / container networking
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Copy the self-contained output (includes RunxRun.dll, the bundled runtime, and wwwroot/)
COPY --from=build /app/publish .

# Set entry point to run your DLL via the self-contained runtime environment
ENTRYPOINT ["dotnet", "RunxRun.dll"]