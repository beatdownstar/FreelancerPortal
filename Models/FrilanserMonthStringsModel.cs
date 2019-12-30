using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ProsjektoppgaveITE1811Gruppe7.Models
{
    public class FrilanserMonthStringsModel
    {
        public string FrilanserId { get; set; }

        public string Period { get; set; }

        [DisplayName("Number Of Strings")]
        public int NumberOfStrings { get; set; }

        [DisplayName("Amount (Litecoins)")]
        public double Amount { get; set; }

        public bool Status { get; set; }
    }
}
