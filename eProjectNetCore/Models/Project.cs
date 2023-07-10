using System;
using System.Collections.Generic;

namespace eProjectNetCore.Models
{
    public partial class Project
    {
        public string Id { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string AccountId { get; set; }
        public double? Price { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Status { get; set; }
        public string CompetitionId { get; set; }

        public Account Account { get; set; }
        public Competition Competition { get; set; }
    }
}
