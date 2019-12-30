using Microsoft.AspNetCore.Identity;
using ProsjektoppgaveITE1811Gruppe7.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProsjektoppgaveITE1811Gruppe7.Models.ViewModels
{
    public class OrderDetails
    {
        public int OrderId { get; set; }

        [DisplayName("Date of creation")]
        public DateTime DateCreated { get; set; }

        [DisplayName("Deadline")]
        public DateTime DateOfDeadline { get; set; }

        [StringLength(50)]
        [DisplayName("Order name")]
        public string OrderName { get; set; }

        [StringLength(1000)]
        [DisplayName("Order description")]
        public string OrderTask { get; set; }

        [DisplayName("Number of strings")]
        public int NumberOfStrings { get; set; }

        public int Status { get; set; }

        public int SolutionFileId { get; set; }

        public List<string> SolutionStrings { get; set; }
        
        public string ClientId { get; set; }

        public SeniorUtvikler SeniorUtvikler { get; set; }

        public Frilansers Frilanser { get; set; }
        [DisplayName("Spesialization")]
        public string SpecializationRoleName { get; set; }

        public OrderDetails()
        {
            Frilanser = new Frilansers();
            SeniorUtvikler = new SeniorUtvikler();

        }
    }
}