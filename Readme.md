# RabbitMQ Playground

This repository demonstrates a simple event-driven system using RabbitMQ, Docker, and .NET Core Minimal APIs. This setup includes a service for sending event logs and a server to manage and dispatch these events.

### Projects Details 

1. **ConsoleAppRabbitMqDemo**
   - .NET Core console application that acts as a service. It sends event logs to RabbitMQ.

2. **WebApplicationRabbitMqDemo**
   - .NET Core web application that serves as the server. It manages the events and sends them to clients.
   - You can browse the web application at [http://localhost:5000/index.html](http://localhost:5000/index.html). This page provides a simple interface to interact with the application.


## Getting Started

Follow these steps to get the RabbitMQ Playground up and running:

### Prerequisites

- [Docker](https://www.docker.com/get-started) (for running RabbitMQ and the web application)
- [.NET 8 SDK](https://dotnet.microsoft.com/download) (or the version you are using)

### Setup RabbitMQ

1. **Start RabbitMQ using Docker**

   Make sure Docker is installed and running on your machine. Then, navigate to the root directory and run:

   ```bash
   docker-compose up --build
   ```

   This will start RabbitMQ and the web application service.

2. **Verify RabbitMQ is Running**

   Once the Docker containers are up, you can access RabbitMQ's management interface by visiting [http://localhost:15672](http://localhost:15672). Default credentials are:
   - **Username:** guest
   - **Password:** guest

### Health Check

The project includes a `wait-for-rabbitmq.sh` script to ensure RabbitMQ is available before starting the web application. This script checks the health of RabbitMQ and waits for it to be ready.

- **Health Check Script (`wait-for-rabbitmq.sh`):** This script attempts to connect to RabbitMQ’s health check endpoint. If RabbitMQ is not ready, it will retry up to a maximum number of times before exiting with an error.
 

  The script will wait for RabbitMQ to be healthy before running the main application command. If RabbitMQ is not available after several retries, it will exit with an error.

### Running the Applications

1. **Run the Console Application**

   Open a separate terminal and navigate to the `ConsoleAppRabbitMqDemo` directory. Then, run:

   ```bash
   dotnet run
   ```

   This application will start and enter a message to send event logs to RabbitMQ.

### Contributing

Feel free to open issues or submit pull requests if you have suggestions or improvements!

---

Feel free to adjust any specific details or configurations based on your actual setup or preferences.
