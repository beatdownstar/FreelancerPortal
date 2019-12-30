using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ProsjektoppgaveITE1811Gruppe7.Models.ViewModels
{
    public class FrilanserDetailsViewModel
    {
        [DisplayName("Frilanser name")]
        public string FrilanserName { get; set; }

        [DisplayName("Balance of LiteCoins in HIN")]
        public string Balanсe { get; set; }

        [DisplayName("Number of strings for client")]
        public List<ClientSumStringsModel> ClientSumStrings { get; set; }

        [DisplayName("Number of strings for month")]
        public List<FrilanserMonthStringsModel> FrilanserMonthStrings { get; set; }

    }
}
