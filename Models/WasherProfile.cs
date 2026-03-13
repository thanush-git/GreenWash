using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenWash.Models
{
    public class WasherProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long WasherId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }

        public double AverageRating { get; set; } = 0;
        public int TotalWashes { get; set; } = 0;
        public bool IsAvailable { get; set; } = true;
    }
}
