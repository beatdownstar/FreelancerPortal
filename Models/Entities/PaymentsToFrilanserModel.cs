using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProsjektoppgaveITE1811Gruppe7.Models.Entities
{
    public class PaymentsToFrilanserModel
    {
        [Key]
        public int PaymentId { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Period")]
        public DateTime Period { get; set; }
                
        [DisplayName("Summ")]
        public double Amount { get; set; }

        [DisplayName("Status")]
        public bool Status { get; set; }

        public virtual IdentityUser Frilanser { get; set; }

    }
}
