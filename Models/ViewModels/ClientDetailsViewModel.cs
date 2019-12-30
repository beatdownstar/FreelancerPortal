using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ProsjektoppgaveITE1811Gruppe7.Models.ViewModels
{
    public class ClientDetailsViewModel
    {
        [DisplayName("Name")]
        public string ClientName { get; set; }

        [DisplayName("Number of strings")]
        public List<FrilanserSumStringModel> FrilanserSumStrings { get; set; }
    }
}
