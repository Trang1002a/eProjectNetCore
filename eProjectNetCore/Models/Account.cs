using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace eProjectNetCore.Models
{
    public partial class Account
    {
        public Account()
        {
            Project = new HashSet<Project>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        [Column("class_id")]
        public string ClassId { get; set; }
        public string Status { get; set; }
        public DateTime? Birthday { get; set; }
        [Column("created_date")]
        public DateTime? CreatedDate { get; set; }
        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        public Class Class { get; set; }
        public ICollection<Project> Project { get; set; }
    }
}
