using System;
using System.Collections.Generic;

namespace eProjectNetCore.Models
{
    public partial class UserGroup
    {
        public UserGroup()
        {
            Menu = new HashSet<Menu>();
            User = new HashSet<User>();
        }

        public byte Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }

        public ICollection<Menu> Menu { get; set; }
        public ICollection<User> User { get; set; }
    }
}
