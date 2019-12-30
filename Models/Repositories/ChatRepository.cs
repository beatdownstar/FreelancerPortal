using Microsoft.AspNetCore.Identity;
using ProsjektoppgaveITE1811Gruppe7.Data;
using ProsjektoppgaveITE1811Gruppe7.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ProsjektoppgaveITE1811Gruppe7.Models
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _manager;

        public ChatRepository(UserManager<IdentityUser> manager, ApplicationDbContext db)
        {
            _db = db;
            _manager = manager;
        }

        public IEnumerable<ChatMessageModel> GetMessagesByOrderId(int id)
        {

            IEnumerable<ChatMessageModel> messages = _db.Messages
                                                        .Where(m => m.OrderId == id);
            return messages;
        }

        public async Task CreateMessage(ChatMessageModel message, IPrincipal principal)
        {

            IdentityUser currentUser = await _manager.FindByNameAsync(principal.Identity.Name);
            message.DateCreated = DateTime.Now;
            message.Owner = currentUser;
            _db.Messages.Add(message);
            _db.SaveChanges();
        }
    }
}
