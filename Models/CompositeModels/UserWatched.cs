using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOFTITOFLIX.Models.CompositeModels
{
	public class UserWatched
	{

        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public SOFTITOFLIXUser? ITOFLIXUser { get; set; }

        public long EpisodeId { get; set; }
        [ForeignKey("EpisodeId")]
        public Episode? Episode { get; set; }
    }
}

