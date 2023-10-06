using System;
using System.Collections.Generic;

namespace DxChinook.Data.EF.Models
{
    public partial class MediaType
    {
        public MediaType()
        {
            Tracks = new HashSet<Track>();
        }

        public int MediaTypeId { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Track> Tracks { get; set; }
    }
}
