using MovieAPI.Entity;

namespace MovieAPI.Repository;

public class InMemoryRepository: IInMemoryRepository
{
    private List<Genre>  _genres;

    public InMemoryRepository()
    {
        _genres = new List<Genre>()
        {
            new Genre() { id = 1, name = "Comedy" },
            new Genre() { id = 2, name = "Action" }
        };
        
    }

    public List<Genre> GetGenres()
    {
        return _genres;
    }

    public async Task<Genre> GetGenreById(int id)
    {
        await Task.Delay(1);
        return _genres.FirstOrDefault(i => i.id == id);
    }
}