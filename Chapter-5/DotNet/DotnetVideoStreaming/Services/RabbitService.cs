using RabbitMQ.Client;
using System;
using System.Text;

namespace Services
{
    public class RabbitService : IRabbitService
    {
        private IConnection _connection;

        public RabbitService()
        {
            CreateConnection();
        }

        private void CreateConnection()
        {
            try
            {
                string RABBIT = Environment.GetEnvironmentVariable("RABBIT");

                var factory = new ConnectionFactory();
                factory.Uri = new Uri(RABBIT);

                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not create connection: {ex.Message}");
            }
        }

        private bool IsConnectionExists()
        {
            if (_connection != null)
            {
                return true;
            }

            CreateConnection();

            return _connection != null;
        }

        public void SendMessage()
        {
            if (IsConnectionExists())
            {
                using (var channel = _connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "RabbitQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                    string message = "Send single message from Video Streaming!";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: "RabbitQueue",
                                         basicProperties: null,
                                         body: body);

                    Console.WriteLine($"{message}");

                }
            }
        }

        public void SendMultipleMessages()
        {
            if (IsConnectionExists())
            {
                using (var channel = _connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "RabbitExchange", type: ExchangeType.Fanout);

                    var message = "Send multiple messages from Video Streaming!";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "RabbitExchange",
                                         routingKey: "",
                                         basicProperties: null,
                                         body: body);

                    Console.WriteLine($"{message}");
                }
            }
        }
    }
}