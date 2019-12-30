using ProsjektoppgaveITE1811Gruppe7.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ProsjektoppgaveITE1811Gruppe7.Models
{
    public interface IChatRepository
    {
        IEnumerable<ChatMessageModel> GetMessagesByOrderId(int id);

        Task CreateMessage(ChatMessageModel message, IPrincipal principal);
    }
  
}
