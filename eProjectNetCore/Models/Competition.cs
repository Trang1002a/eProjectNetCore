using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace eProjectNetCore.Models
{
    public partial class Competition
    {
        public Competition()
        {
            Project = new HashSet<Project>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        [Column("start_date")]
        public DateTime? StartDate { get; set; }
        [Column("end_date")]
        public DateTime? EndDate { get; set; }
        public string Prize { get; set; }
        public string Status { get; set; }

        public ICollection<Project> Project { get; set; }
    }
}
