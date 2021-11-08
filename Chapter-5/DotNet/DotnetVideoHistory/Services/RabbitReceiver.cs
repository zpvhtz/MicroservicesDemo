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
        //private readonly IMessageService _messageService;

        //public Receiver(IMessageService messageService)
        public RabbitReceiver()
        {
            //_messageService = messageService;
            InitializeRabbitMqListener();
            //InitializeRabbitMqListenerExchange();
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

        //Multiple messages
        private void InitializeRabbitMqListenerExchange()
        {
            string RABBIT = Environment.GetEnvironmentVariable("RABBIT");

            var factory = new ConnectionFactory() { Uri = new Uri(RABBIT) };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "RabbitExchange", type: ExchangeType.Fanout);

            queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: queueName,
                               exchange: "RabbitExchange",
                               routingKey: "");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());

                Console.WriteLine($"Content: {content} from History");
                //_messageService.Add(new Message { NoiDung = content });
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queueName, false, consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}