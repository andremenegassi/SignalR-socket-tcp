using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalRTeste
{
    public class Escuta: Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("Send", $"{Context.ConnectionId} joined");
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Clients.Others.SendAsync("Send", $"{Context.ConnectionId} left");
        }

        public Task Send(string message)
        {
            return Clients.All.SendAsync("Send", $"{Context.ConnectionId}: {message}");
        }

        public Task SendToOthers(string message)
        {
            return Clients.Others.SendAsync("Send", $"{Context.ConnectionId}: {message}");
        }

        public Task SendToConnection(string connectionId, string message)
        {
            return Clients.Client(connectionId).SendAsync("Send", $"Private message from {Context.ConnectionId}: {message}");
        }

        public Task SendToGroup(string groupName, string message)
        {
            return Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId}@{groupName}: {message}");
        }

        public Task SendToOthersInGroup(string groupName, string message)
        {
            return Clients.OthersInGroup(groupName).SendAsync("Send", $"{Context.ConnectionId}@{groupName}: {message}");
        }


        public Task Echo(string message)
        {
            return Clients.Caller.SendAsync("Send", $"{Context.ConnectionId}: {message}");
        }

    }


     
}
