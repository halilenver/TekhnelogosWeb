using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TekhnelogosWeb.Hubs;
using TekhnelogosWeb.Models;

namespace TekhnelogosWeb.Services
{
    public delegate void MessageReceivedEventHandler(string message);
    public class Worker : IHostedService, IDisposable
    {
        private IConnection connection;
        private IModel channel;
        private readonly IOptions<RabbitMQConfiguration> _config;
        private readonly ILogger<Worker> _logger;
        private readonly MessageHub _hub;

        private event MessageReceivedEventHandler _messageReceived;
        public event MessageReceivedEventHandler MessageReceived
        {
            add
            {
                _messageReceived = value;
            }
            remove
            {
                _messageReceived -= value;
            }
        }

        public Worker(ILogger<Worker> logger, IOptions<RabbitMQConfiguration> config, MessageHub hub)
        {
            _config = config;
            _hub = hub;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _config.Value.HostName,
                    UserName = _config.Value.Username,
                    Password = _config.Value.Password
                };

                connection = factory.CreateConnection();

                channel = connection.CreateModel();
                channel.ExchangeDeclare(_config.Value.Exchange, ExchangeType.Fanout);
                channel.QueueBind(queue: _config.Value.Queue, exchange: _config.Value.Exchange, routingKey: _config.Value.RoutingKey);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    if (_messageReceived == null)
                        _messageReceived += Customer_MessageReceived;
                    _messageReceived?.Invoke(message);
                };

                channel.BasicConsume(queue: _config.Value.Queue, autoAck: true, consumer: consumer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return Task.FromResult(0);
        }

        private async void Customer_MessageReceived(string message)
        {
            try
            {
                //calculate vowels
                int vowelCount = message == null ? 0 : Regex.Matches(message, @"[AEIİOÖUÜaeıioöuü]").Count;

                //send via signalR
                await _hub.SendMessage(message, vowelCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
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
