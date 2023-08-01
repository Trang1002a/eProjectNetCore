using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace eProjectNetCore.Models
{
    public partial class UserGroup
    {
        public UserGroup()
        {
            User = new HashSet<User>();

            Package = new HashSet<Package>();
        }

        public byte Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        
        public ICollection<User> User { get; set; }
        public ICollection<Package> Package { get; set; }
    }
}
