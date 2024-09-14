using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    // channel.QueueDeclare(queue: "messages",
    //     durable: false,
    //     exclusive: false,
    //     autoDelete: false,
    //     arguments: null);

    channel.ExchangeDeclare("logs", ExchangeType.Fanout);

    while (true)
    {
        // Console.WriteLine("Enter a message to send:");
        var message = GetMessage(args);
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: "logs",
                             routingKey: string.Empty,
                             basicProperties: null,
                             body: body);

        // channel.BasicPublish(exchange: "",
        //     routingKey: "messages",
        //     basicProperties: null,
        //     body: body);
        // Console.WriteLine(" [x] Sent {0}", message);
        Task.Delay(TimeSpan.FromSeconds(1)).GetAwaiter().GetResult();
    }
}

static string GetMessage(string[] args)
{
    return (args.Length > 0) ? string.Join(" ", args) : "info: Hello World!";
}