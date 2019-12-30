using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProsjektoppgaveITE1811Gruppe7.Models.ViewModels
{
    public class NumberOfStringsForClientModel
    {
        public IdentityUser Client { get; set; }

        public int NumberOfStrings { get; set; }

        public int NumberOfStingsInMonth { get; set; }
    }
}
