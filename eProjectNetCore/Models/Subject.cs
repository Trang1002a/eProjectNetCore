using System;
using System.Collections.Generic;

namespace eProjectNetCore.Models
{
    public partial class Subject
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public int? Session { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
