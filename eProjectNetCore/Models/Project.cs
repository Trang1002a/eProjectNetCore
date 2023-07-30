using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace eProjectNetCore.Models
{
    public partial class Project
    {
        public string Id { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string AccountId { get; set; }
        public double? Price { get; set; }
        [Column("created_date")]
        public DateTime? CreatedDate { get; set; }
        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }
        public string Status { get; set; }
        public string CompetitionId { get; set; }
        public string Comment { get; set; }
        public int? Mark { get; set; }
        public string UserId { get; set; }
        public Account Account { get; set; }
        public Competition Competition { get; set; }
        public User User { get; set; }
    }
}
