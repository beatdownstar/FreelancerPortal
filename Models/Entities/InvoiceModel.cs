using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProsjektoppgaveITE1811Gruppe7.Models.Entities
{
    public class InvoiceModel
    {
        [Key]
        public int id { get; set; }

        public Boolean Status { get; set; }

        public double Amount { get; set; }

        public virtual Order order { get; set; }
}
}
