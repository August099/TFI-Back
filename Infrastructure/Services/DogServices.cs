using Application.Interfaces;
using Application.Models;
using Application.Models.Dog;
using Infrastructure.Models;
using System.Net;
using System.Net.Http.Json;

namespace Infrastructure.Services
{

    public class DogServices : IDogServices
    {
        private readonly HttpClient _httpClient;

        public DogServices(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("dogs");
        }

        public async Task<List<BreedDTO>> GetBreedListAsync()
        {
            var response = await _httpClient
                .GetFromJsonAsync<DogApiResponse<Dictionary<string, List<string>>>>("breeds/list/all");

            return response?.Message.Select(kv => new BreedDTO { Name = kv.Key, SubBreeds = kv.Value }).ToList() ?? [];
        }

        public async Task<List<SubBreedDTO>> GetSubBreedListAsync(string breed)
        {
            var response = await _httpClient
                .GetFromJsonAsync<DogApiResponse<List<string>>>($"breed/{breed}/list");

            return response?.Message.Select(subBreed => new SubBreedDTO { Name = subBreed }).ToList() ?? [];
        }

        public async Task<DogImageDTO?> GetRandomDogAsync()
        {
            var response = await _httpClient
                .GetFromJsonAsync<DogApiResponse<string>>("breeds/image/random");

            return new DogImageDTO { Url = response?.Message ?? string.Empty };
        }

        public async Task<List<DogImageDTO>> GetMultipleRandomDogAsync (int cantidad)
        {
            var response = await _httpClient
                .GetFromJsonAsync<DogApiResponse<List<string>>>($"breeds/image/random/{cantidad}");

            return response?.Message.Select(url => new DogImageDTO { Url = url }).ToList() ?? [];
        }

        public async Task<List<DogImageDTO>> GetDogsByBreedAsync(string breed)
        {
            var response = await _httpClient
                .GetFromJsonAsync<DogApiResponse<List<string>>>($"breed/{breed}/images");

            return response?.Message.Select(url => new DogImageDTO { Url = url }).ToList() ?? [];
        }

        public async Task<DogImageDTO> GetRandomDogByBreedAsync(string breed)
        {
            var response = await _httpClient
                .GetFromJsonAsync<DogApiResponse<string>>($"breed/{breed}/images/random");

            return new DogImageDTO { Url = response?.Message ?? string.Empty };
        }

        public async Task<List<DogImageDTO>> GetMultipleRandomDogByBreedAsync(string breed, int cantidad)
        {
            var response = await _httpClient
                .GetFromJsonAsync<DogApiResponse<List<string>>>($"breed/{breed}/images/random/{cantidad}");

            return response?.Message.Select(url => new DogImageDTO { Url = url }).ToList() ?? [];
        }

        public async Task<List<DogImageDTO>> GetDogsBySubBreedAsync(string breed, string subBreed)
        {
            var response = await _httpClient
                .GetFromJsonAsync<DogApiResponse<List<string>>>($"breed/{breed}/{subBreed}/images");

            return response?.Message.Select(url => new DogImageDTO { Url = url }).ToList() ?? [];
        }

        public async Task<DogImageDTO> GetRandomDogBySubBreedAsync(string breed, string subBreed)
        {
            var response = await _httpClient
                .GetFromJsonAsync<DogApiResponse<string>>($"breed/{breed}/{subBreed}/images/random");

            return new DogImageDTO { Url = response?.Message ?? string.Empty };
        }

        public async Task<List<DogImageDTO>> GetMultipleRandomDogBySubBreedAsync(string breed, string subBreed, int cantidad)
        {
            var response = await _httpClient
                .GetFromJsonAsync<DogApiResponse<List<string>>>($"breed/{breed}/{subBreed}/images/random/{cantidad}");

            return response?.Message.Select(url => new DogImageDTO { Url = url }).ToList() ?? [];
        }
        
    }
}
