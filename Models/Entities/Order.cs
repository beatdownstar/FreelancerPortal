using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProsjektoppgaveITE1811Gruppe7.Models.Entities
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [DisplayName("Date of creation")]
        public DateTime DateCreated { get; set; }
        [DataType(DataType.Date)]

        [DisplayName("Deadline")]
        [Required]
        public DateTime DateOfDeadline { get; set; }

        [StringLength(50)]
        [DisplayName("Order name")]
        [Required]
        public string OrderName { get; set; }
        [StringLength(1000)]
        [DisplayName("Order description")]
        [Required]
        public string OrderTask { get; set; }

        [StringLength(50)]
        public string Solution { get; set; }

        public int NumberOfStrings { get; set; }

        public int Status { get; set; }

        public virtual FileModel SolutionFile { get; set; } 

        public virtual IdentityUser Client { get; set; }

        public virtual IdentityUser SeniorUtvikler { get; set; }

        public virtual IdentityUser Frilanser { get; set; }

        public virtual IdentityRole SpecializationRole { get; set; }
    }
}