using ProsjektoppgaveITE1811Gruppe7.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ProsjektoppgaveITE1811Gruppe7.Models.ViewModels
{
    public class ClientsInvoicesViewModel
    {
        public List<InvoiceModel> invoices { get; set; }
        [DisplayName("Price for string")]
        public double PriceForString  { get; set; }
        
    }
}
