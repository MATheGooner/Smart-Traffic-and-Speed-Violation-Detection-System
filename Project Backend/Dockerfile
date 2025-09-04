# Use the official .NET SDK to build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Use a smaller runtime image for final container
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out .

# Expose port 7255 for Railway (you can also expose 443 if using HTTPS)
EXPOSE 7255

# Start the app
ENTRYPOINT ["dotnet", "Traffic Control System.dll"]
