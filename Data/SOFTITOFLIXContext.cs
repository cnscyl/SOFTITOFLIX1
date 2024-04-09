using System;
using Microsoft.EntityFrameworkCore;
using SOFTITOFLIX.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SOFTITOFLIX.Data
{
	public class SOFTITOFLIXContext : IdentityDbContext<SOFTITOFLIXUser,SOFTITOFLIXRole, long>
	{
		public SOFTITOFLIXContext(DbContextOptions<SOFTITOFLIXContext> options): base(options)
		{
		}
	

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<SOFTITOFLIX.Models.CompositeModels.MediaCategory>().HasKey(mc => new { mc.MediaId, mc.CategoryId });
            builder.Entity<SOFTITOFLIX.Models.CompositeModels.MediaDirector>().HasKey(md => new { md.MediaId, md.DirectorId });
            builder.Entity<SOFTITOFLIX.Models.CompositeModels.MediaRestriction>().HasKey(mr => new { mr.MediaId, mr.RestrictionId });
            builder.Entity<SOFTITOFLIX.Models.CompositeModels.MediaActor>().HasKey(ma => new { ma.MediaId, ma.ActorId });
            builder.Entity<SOFTITOFLIX.Models.CompositeModels.UserFavorite>().HasKey(uf => new { uf.MediaId, uf.UserId });
            builder.Entity<SOFTITOFLIX.Models.CompositeModels.UserWatched>().HasKey(uw => new { uw.EpisodeId, uw.UserId });
            builder.Entity<SOFTITOFLIX.Models.Episode>().HasIndex(e => new { e.MediaId, e.EpisodeNumber, e.SeasonNumber }).IsUnique(true);
            builder.Entity<SOFTITOFLIX.Models.SOFTITOFLIXUser>().HasIndex(user => user.Email).IsUnique(true);
        }
        public DbSet<SOFTITOFLIX.Models.Actor> Actors { get; set; } = default!;
        public DbSet<SOFTITOFLIX.Models.Category> Categories { get; set; } = default!;
        public DbSet<SOFTITOFLIX.Models.Director> Directors { get; set; } = default!;
        public DbSet<SOFTITOFLIX.Models.Episode> Episodes { get; set; } = default!;
        public DbSet<SOFTITOFLIX.Models.Media> Media { get; set; } = default!;
        public DbSet<SOFTITOFLIX.Models.Plan> Plans { get; set; } = default!;
        public DbSet<SOFTITOFLIX.Models.Restriction> Restrictions { get; set; } = default!;
        public DbSet<SOFTITOFLIX.Models.UserSubscription> UserSubscriptions { get; set; } = default!;

        public DbSet<SOFTITOFLIX.Models.CompositeModels.MediaCategory> MediaCategories { get; set; } = default!;
        public DbSet<SOFTITOFLIX.Models.CompositeModels.MediaDirector> MediaDirectors { get; set; } = default!;
        public DbSet<SOFTITOFLIX.Models.CompositeModels.MediaRestriction> MediaRestrictions { get; set; } = default!;
        public DbSet<SOFTITOFLIX.Models.CompositeModels.MediaActor> MediaActors { get; set; } = default!;
        public DbSet<SOFTITOFLIX.Models.CompositeModels.UserFavorite> UserFavorites { get; set; } = default!;
        public DbSet<SOFTITOFLIX.Models.CompositeModels.UserWatched> UserWatcheds { get; set; } = default!;

    }
}

