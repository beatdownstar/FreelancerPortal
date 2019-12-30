using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace ProsjektoppgaveITE1811Gruppe7.Hubs

{
    public class ChatHub : Hub
    {
        public async Task UpdateForNewMessages(string message, int messageId, int orderId, string userName)
        {
            DateTime dateCreated = DateTime.Now;
            await Clients.All.SendAsync("UpdateMessages", dateCreated, message, messageId, orderId, userName);
        }
               
    }
}
