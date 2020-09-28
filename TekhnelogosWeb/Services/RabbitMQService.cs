using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TekhnelogosWeb.Models;

namespace TekhnelogosWeb.Services
{
    public class RabbitMQService : IDisposable
    {
        private IConnection connection;
        private IModel channel;
        private readonly IOptions<RabbitMQConfiguration> _config;
        private readonly ILogger<RabbitMQService> _logger;

        public RabbitMQService(ILogger<RabbitMQService> logger, IOptions<RabbitMQConfiguration> config)
        {
            try
            {
                _logger = logger;
                _config = config;

                var factory = new ConnectionFactory
                {
                    HostName = _config.Value.HostName,
                    UserName = _config.Value.Username,
                    Password = _config.Value.Password
                };

                connection = factory.CreateConnection();

                channel = connection.CreateModel();
                channel.ExchangeDeclare(_config.Value.Exchange, ExchangeType.Fanout);   
                channel.QueueDeclare(queue: _config.Value.Queue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public void Send(string message)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: _config.Value.Exchange, routingKey: _config.Value.RoutingKey, basicProperties: null, body: body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public void Dispose()
        {
            if (channel != null)
                channel.Dispose();
            if (connection != null)
                connection.Dispose();
        }
    }
}
