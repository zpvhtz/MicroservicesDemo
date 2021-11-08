using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Data.Models
{
    public partial class Videos : BaseEntity
    {
        //public int Id { get; set; }
        public string Path { get; set; }
    }
}
