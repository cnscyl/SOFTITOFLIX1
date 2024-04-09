using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOFTITOFLIX.Models.CompositeModels
{
	public class UserFavorite
	{
        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public SOFTITOFLIXUser? ITOFLIXUser { get; set; }

        public int MediaId { get; set; }
        [ForeignKey("MediaId")]
        public Media? Media { get; set; }
    }
}

