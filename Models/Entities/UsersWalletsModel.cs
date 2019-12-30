using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProsjektoppgaveITE1811Gruppe7.Models.Entities
{
    public class UsersWalletsModel
    {
        [Key]
        public int Id { get; set; }
        public string Address { get; set; }
        public virtual IdentityUser User { get; set; }
    }
}
