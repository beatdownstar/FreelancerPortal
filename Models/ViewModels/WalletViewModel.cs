using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace ProsjektoppgaveITE1811Gruppe7.Models.ViewModels
{
    public class WalletViewModel
    {
        [DisplayName("Balance")]
        public string Balance { get; set; }
                
        public Dictionary<long, string> Transactions { get; set; }
    }
}
