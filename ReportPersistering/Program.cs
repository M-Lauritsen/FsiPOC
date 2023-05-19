using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

Console.ForegroundColor = ConsoleColor.Magenta;
Console.Clear();
var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "FSI", type: ExchangeType.Topic);
// declare a server-named queue
var queueName = channel.QueueDeclare().QueueName;

channel.QueueBind(queue: queueName,
                     exchange: "FSI",
                     routingKey: "report.persistering");


Console.WriteLine(" - Report Persistering Running.");

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var routingKey = ea.RoutingKey;

    Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
};
channel.BasicConsume(queue: queueName,
                     autoAck: true,
                     consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();