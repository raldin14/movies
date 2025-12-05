using MovieAPI.Entity;

namespace MovieAPI.Repository;

public interface IInMemoryRepository
{
    List<Genre> GetGenres();
    Task<Genre> GetGenreById(int id);
}