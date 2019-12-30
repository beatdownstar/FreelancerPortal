using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProsjektoppgaveITE1811Gruppe7.Models.ViewModels
{
    public class UploadSolutionViewModel
    {
        public int OrderId { get; set; }
        
        [StringLength(50)]
        [DisplayName("Order name")]
        public string OrderName { get; set; }
        [StringLength(1000)]
        [DisplayName("Order description")]
        public string OrderTask { get; set; }

       
        public string Solution { get; set; }

        public IFormFile uploadedFile { get; set; }
        [DisplayName("Clients name")]
        public string ClientName { get; set; }
        [DisplayName("Deadline")]
        public DateTime DateOfDeadline { get; set; }

    }
}
