using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Entity;

namespace MovieAPI;

public class ApplicationDBContext: IdentityDbContext
{
    public ApplicationDBContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MoviesActors>()
            .HasKey(x => new {x.actorId, x.movieId });
        modelBuilder.Entity<MoviesGenres>()
            .HasKey(x => new {x.movieId , x.genreId });
        modelBuilder.Entity<MoviesTheathers>()
            .HasKey(x => new { x.movieId, x.theaterId });
        
        base.OnModelCreating(modelBuilder);
    }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<Theather>  Theathers { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<MoviesActors> MoviesActors { get; set; }
    public DbSet<MoviesGenres> MoviesGenres { get; set; }
    public DbSet<MoviesTheathers> MoviesTheathers { get; set; }
    public DbSet<Rating> Ratings { get; set; }
}