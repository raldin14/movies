namespace MovieAPI.DTOs;

public class PaginationDTO
{
    public int page { get; set; } = 1;
    private int _pageSize = 10;
    private readonly int _maxPageSize = 50;

    public int pageSize
    {
        get{return _pageSize;}
        set
        {
            _pageSize = (value > _maxPageSize) ? _maxPageSize : value;
        }
    }
}