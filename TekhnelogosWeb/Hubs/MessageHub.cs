using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TekhnelogosWeb.Hubs
{
    public class MessageHub : Hub
    {
        public async Task SendMessage(string message, int vowelc)
        {
            await Clients.All.SendAsync("ReceiveMessage", message, vowelc);
        }
    }
}
