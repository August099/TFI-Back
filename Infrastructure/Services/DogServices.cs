using Application.Interfaces;
using Application.Models.Dog;
using Infrastructure.Models;
using System.Net.Http.Json;

namespace Infrastructure.Services
{

    public class DogServices : IDogServices
    {
        private readonly HttpClient _httpClient;

        private static Dictionary<string, List<string>>? _sharedBreedsCache;

        public DogServices(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("dogs");
        }

        public async Task<bool> BreedExistsAsync(string breed)
        {
            // si todavia no se cargaron las razas llama a la api
            if (_sharedBreedsCache == null)
            {
                var response = await _httpClient
                    .GetFromJsonAsync<DogApiResponse<Dictionary<string, List<string>>>>("breeds/list/all");

                _sharedBreedsCache = response?.Message ?? new Dictionary<string, List<string>>();
            }

            // valida si la raza existe en el diccionario
            return _sharedBreedsCache.ContainsKey(breed.ToLower());
        }

        public async Task<bool> SubBreedExistsAsync(string breed, string subBreed)
        {
            // si todavia no se cargaron las razas llama a la api
            if (_sharedBreedsCache == null)
            {
                var response = await _httpClient.GetFromJsonAsync<DogApiResponse<Dictionary<string, List<string>>>>("breeds/list/all");
                _sharedBreedsCache = response?.Message ?? new Dictionary<string, List<string>>();
            }

            // normalizamos los nombres
            string normalizedBreed = breed.ToLower();
            string normalizedSubBreed = subBreed.ToLower();

            // valida si la raza existe en el diccionario
            if (!_sharedBreedsCache.ContainsKey(normalizedBreed))
            {
                return false;
            }

            // obtiene la lista de subrazas
            List<string> subBreedsList = _sharedBreedsCache[normalizedBreed];

            // valida si la subraza existe en la lista de subrazas
            return subBreedsList.Contains(normalizedSubBreed);
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
