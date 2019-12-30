using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProsjektoppgaveITE1811Gruppe7.Models.ViewModels
{
    public class NewOrderViewModel
    {
        [DataType(DataType.Date)]
        [DisplayName("Deadline")]
        [Required]
        public DateTime DateOfDeadline { get; set; }

        [DisplayName("Order name")]
        [StringLength(50)]
        [Required]
        public string OrderName { get; set; }

        [DisplayName("Order description")]
        [StringLength(1000)]
        [Required]
        public string OrderTask { get; set; }

        [DisplayName("Frilanser")]
        public string FrilanserId { get; set; }

        [DisplayName("Spesialization")]
        public string SpecializationRoleId { get; set; }

        public List<Frilansers> Frilansers { get; set; }

        public List<IdentityRole> SpecializationRoles { get; set; }
        


    }
}