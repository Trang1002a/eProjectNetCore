using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace eProjectNetCore.Models
{
    public partial class Class
    {
        public Class()
        {
            Account = new HashSet<Account>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        [Column("created_date")]
        public DateTime? CreatedDate { get; set; }
        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        public ICollection<Account> Account { get; set; }
    }
}
