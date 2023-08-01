using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace eProjectNetCore.Models
{
    public partial class Package
    {
        public byte Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public byte? GroupId { get; set; }
        [Column("menu_id")]
        public string MenuId { get; set; }
        [Column("created_date")]
        public DateTime? CreatedDate { get; set; }
        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }
        public UserGroup Group { get; set; }
    }
}
