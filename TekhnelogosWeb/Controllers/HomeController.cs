using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using TekhnelogosWeb.Hubs;
using TekhnelogosWeb.Models;
using TekhnelogosWeb.Services;

namespace TekhnelogosWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly RabbitMQService _producer;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, RabbitMQService producer)
        {
            _logger = logger;
            _producer = producer;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public void SendWord([FromBody]WordMessage word) => _producer.Send(word.Message);

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
