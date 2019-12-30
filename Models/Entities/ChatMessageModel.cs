using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProsjektoppgaveITE1811Gruppe7.Models.Entities
{
    public class ChatMessageModel
    {
        [Key]
        public int MessageId { get; set; }
        public DateTime DateCreated { get; set; }
        public int OrderId { get; set; }
        [Required(ErrorMessage = "Message is empty, write something")]
        public string Content { get; set; }
        public virtual IdentityUser Owner { get; set; }

    }
}