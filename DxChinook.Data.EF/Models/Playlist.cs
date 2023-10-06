using System;
using System.Collections.Generic;

namespace DxChinook.Data.EF.Models
{
    public partial class Playlist
    {
        public Playlist()
        {
            Tracks = new HashSet<Track>();
        }

        public int PlaylistId { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Track> Tracks { get; set; }
    }
}
