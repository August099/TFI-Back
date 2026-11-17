namespace Infrastructure.Models
{
    public class DogApiResponse<T>
    {
        public T Message { get; set; } = default!;
        public string Status { get; set; } = string.Empty;
    }
}
