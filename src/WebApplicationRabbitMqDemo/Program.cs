using System.Text;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddHostedService<RabbitMqBackgroundService>();

var app = builder.Build();
app.UseStaticFiles();
app.MapGet("/", () => Results.Redirect("/index.html"));

app.MapHub<NotificationHub>("/notificationHub");
await app.RunAsync();


public class NotificationHub : Hub
{
}


public class RabbitMqBackgroundService : BackgroundService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName = string.Empty;
    public RabbitMqBackgroundService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;

        var factory = new ConnectionFactory() { HostName = "rabbitmq" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        // _channel.QueueDeclare(queue: "messages", durable: false, exclusive: false, autoDelete: false, arguments: null);
        _channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);
        // declare a server-named queue
        _queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue: _queueName,
                          exchange: "logs",
                          routingKey: string.Empty);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] {message}");
            await _hubContext.Clients.All.SendAsync("receiveMessage", message, stoppingToken);
        };
        _channel.BasicConsume(queue: _queueName,
                             autoAck: true,
                             consumer: consumer);

        // consumer.Received += async (model, ea) =>
        // {
        //     var body = ea.Body.ToArray();
        //     var message = Encoding.UTF8.GetString(body);
        //     await _hubContext.Clients.All.SendAsync("receiveMessage", message, stoppingToken);
        // };

        // _channel.BasicConsume(queue: "messages", autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken stoppingToken)
    {
        _channel.Close();
        _connection.Close();
        return base.StopAsync(stoppingToken);
    }
}