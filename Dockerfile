# Use the official .NET image as a base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Install curl (needed for the wait script) in the base image
RUN apt-get update && apt-get install -y curl

# Copy the wait script into the image
COPY wait-for-rabbitmq.sh /usr/local/bin/wait-for-rabbitmq.sh
RUN chmod +x /usr/local/bin/wait-for-rabbitmq.sh

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY /src/WebApplicationRabbitMqDemo/WebApplicationRabbitMqDemo.csproj WebApplicationRabbitMqDemo/
RUN dotnet restore WebApplicationRabbitMqDemo/WebApplicationRabbitMqDemo.csproj

# Copy the rest of the source code and build the application
COPY /src/WebApplicationRabbitMqDemo/. WebApplicationRabbitMqDemo/
WORKDIR /src/WebApplicationRabbitMqDemo
RUN dotnet build -c Release -o /app/build

# Publish the application to a folder
RUN dotnet publish -c Release -o /app/publish

# Copy the built application to the base image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Ensure the wait script is available and executable
COPY --from=base /usr/local/bin/wait-for-rabbitmq.sh /usr/local/bin/wait-for-rabbitmq.sh

# Use the wait script to delay the start of the app
ENTRYPOINT ["wait-for-rabbitmq.sh", "rabbitmq:5672", "dotnet", "WebApplicationRabbitMqDemo.dll"]
