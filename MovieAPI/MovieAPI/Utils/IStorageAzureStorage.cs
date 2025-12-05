namespace MovieAPI.Utils;

public interface IStorageAzureStorage
{
    Task<string> SaveFile(string container, IFormFile file);
    Task DeleteFile(string container, string route);
    Task<string> EditFile(string container, string route, IFormFile file);
}