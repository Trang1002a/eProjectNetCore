using System;
using System.Collections.Generic;

namespace eProjectNetCore.Models
{
    public partial class Menu
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Status { get; set; }
        public byte? Order { get; set; }
    }
}
