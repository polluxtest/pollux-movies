using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Movies.Application.Services;
using Movies.Domain.Entities;

namespace Movies.Application.ThirdParty
{
    public interface IImbdService
    {
        Task FindImbdMovie();
        Task<string> GetAllMovieRatings(string imbdCode);
        Task SaveMovieRating(Movie movie, string imbdRating);
    }

    public class ImbdService : IImbdService
    {
        private readonly IMoviesServiceAzure movieServiceAzure;
        private readonly IMoviesService movieService;
        private readonly HttpClient client = new HttpClient();

        public ImbdService(IMoviesService movieService, IMoviesServiceAzure moviesServiceAzure)
        {
            this.movieService = movieService;
            this.movieServiceAzure = moviesServiceAzure;
        }

        /// <summary>
        /// Finds the imbd movie.
        /// </summary>
        /// <exception cref="System.ArgumentException">Could not find movie</exception>
        public async Task FindImbdMovie()
        {
            var movies = await this.movieServiceAzure.GetAllAsync();

            foreach (var movie in movies)
            {
                if (!string.IsNullOrEmpty(movie.Imbd)) continue;

                var requestUrl = ImbdApiRouteConstants.GetImbdMovieByName;
                requestUrl = requestUrl + movie.Name;
                var request = this.GetHttpRequest(requestUrl);

                using (var response = await this.client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new ArgumentException("Could not find movie ", movie.Name);
                    }

                    var body = await response.Content.ReadAsStringAsync();
                    var startIndex = body.IndexOf("tt", StringComparison.Ordinal);
                    var imbdCode = body.Substring(startIndex, 9);
                    var imbdRanking = await this.GetAllMovieRatings(imbdCode);
                    await this.SaveMovieRating(movie, imbdRanking);
                }
            }
        }

        /// <summary>
        /// Gets all movie ratings.
        /// </summary>
        /// <param name="imbdCode">The imbd code.</param>
        /// <returns>String.</returns>
        public async Task<string> GetAllMovieRatings(string imbdCode)
        {
            var requestUrl = ImbdApiRouteConstants.GetImbdMovieByCode + imbdCode;
            var request = this.GetHttpRequest(requestUrl);
            var field = "\"rating\":";

            using (var response = await this.client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var startIndex = body.IndexOf(field, StringComparison.Ordinal);
                var rating = body.Substring(startIndex + field.Length, 3);
                return rating;
            }

            throw new ArgumentException("Could not find movie score ", imbdCode);
        }

        /// <summary>
        /// Saves the movie rating.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <param name="imbdRating">The imbd rating.</param>
        /// <returns>Task.</returns>
        public Task SaveMovieRating(Movie movie, string imbdRating)
        {
            movie.Imbd = imbdRating;
            this.movieService.Update(movie);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the HTTP request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>HttpRequestMessage.</returns>
        private HttpRequestMessage GetHttpRequest(string url)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "x-rapidapi-host", ImbdApiConstants.Host },
                    { "x-rapidapi-key", ImbdApiConstants.ClientSecret },
                },
            };

            return request;
        }
    }
}