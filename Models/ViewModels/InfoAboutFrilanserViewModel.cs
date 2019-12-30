using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProsjektoppgaveITE1811Gruppe7.Models.ViewModels
{
    public class InfoAboutFrilanserViewModel
    {
        public Frilansers Frilanser { get; set; }
        public List<string> ClientsOfFrilanser { get; set; }

    }
}
