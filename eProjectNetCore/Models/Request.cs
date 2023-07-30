using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace eProjectNetCore.Models
{
    public partial class Request
    {
        public Request()
        {
            Project = new HashSet<Project>();
        }

        public string Id { get; set; }
        public string CompetitionId { get; set; }
        public string StudentId { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        [Column("created_date")]
        public DateTime? CreatedDate { get; set; }
        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        public Class Class { get; set; }
        public ICollection<Project> Project { get; set; }
    }
}
