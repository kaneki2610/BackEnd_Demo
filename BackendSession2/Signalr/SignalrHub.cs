using BackendSession2.Core.Models;
using BackendSession2.Service;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace BackendSession2.Signalr
{
    public class SignalrHub : Hub<IHubClient>
    {
        public async Task BroadcastMessage(string msg)
        {
            await Clients.All.BroadcastMessage(msg);
        }

    }
    //public class SignalrHub: Hub
    //{
    //    public async Task SendMessageToClient(string message)
    //    {
    //        await Clients.Others.SendAsync("ReceviceMessage", message);
    //        Console.WriteLine($"Receive message from server: ${message}");
    //    }
    //}
}
