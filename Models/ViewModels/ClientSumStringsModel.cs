using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ProsjektoppgaveITE1811Gruppe7.Models.ViewModels
{
    public class ClientSumStringsModel
    {
        [DisplayName("Client name")]
        public string ClientName { get; set; }

        [DisplayName("Number of strings to client")]
        public int SummOfStrings { get; set; }
    }
}
