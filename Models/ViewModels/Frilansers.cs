using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ProsjektoppgaveITE1811Gruppe7.Models.ViewModels
{
    public class Frilansers
    {
        public string Id { get; set; }

        [DisplayName("Frilanser")]
        public string UserName { get; set; }        

    }
}