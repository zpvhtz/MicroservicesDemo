using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class RabbitReceiver : BackgroundService
    {
        private IModel _channel;
        private IConnection _connection;
        private string queueName;

        public RabbitReceiver()
        {
            //InitializeRabbitMqListener();
            InitializeRabbitMqListenerExchange();
        }

        //Single message
        private void InitializeRabbitMqListener()
        {
            string RABBIT = Environment.GetEnvironmentVariable("RABBIT");

            var factory = new ConnectionFactory() { Uri = new Uri(RABBIT) };
            queueName = "RabbitQueue";

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        private void InitializeRabbitMqListenerExchange()
        {
            string RABBIT = Environment.GetEnvironmentVariable("RABBIT");

            var factory = new ConnectionFactory() { Uri = new Uri(RABBIT) };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "RabbitExchange", type: ExchangeType.Fanout);

            queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: queueName, exchange: "RabbitExchange", routingKey: "");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($"Content: {content} from Recommendation");

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queueName, false, consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Dispose();
            _connection.Close();
            base.Dispose();
        }
    }
}