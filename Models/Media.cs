using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOFTITOFLIX.Models
{
	public class Media
	{
		public int Id { get; set; }

		[StringLength(200, MinimumLength =2)]
		[Column(TypeName= "nvarchar(200)")]
		public string Name { get; set; } = "";

		[StringLength(500)]
		[Column(TypeName = "nvarchar(500)")]
		public string? Description { get; set; }

		public bool Passive { get; set; }

		public List<SOFTITOFLIX.Models.CompositeModels.MediaCategory>? MediaCategories { get; set; }

        public List<SOFTITOFLIX.Models.CompositeModels.MediaActor>? MediaActors { get; set; }

        public List<SOFTITOFLIX.Models.CompositeModels.MediaDirector>? MediaDirectors { get; set; }

		public List<SOFTITOFLIX.Models.CompositeModels.MediaRestriction>? MediaRestrictions { get; set; }

		//[Range(0,10)]
		//public float IMDBRating { get; set; }
	}
}

