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

namespace TekhnelogosWeb
{
    public class RabbitMQConfiguration
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string HostName { get; set; }
        public string Queue { get; set; }
        public string RoutingKey { get; set; }
        public string Exchange { get; set; }
    }
}
