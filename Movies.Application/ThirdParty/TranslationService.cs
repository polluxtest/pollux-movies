using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Movies.Application.Services;

namespace Movies.Application.ThirdParty
{
    public interface ITranslationService
    {
        Task TranslateEnToEs();
    }

    public class TranslationService : ITranslationService
    {
        private readonly IMoviesServiceAzure moviesServiceAzure;
        private readonly IMoviesService moviesService;

        public TranslationService(IMoviesServiceAzure moviesServiceAzure, IMoviesService moviesService)
        {
            this.moviesServiceAzure = moviesServiceAzure;
            this.moviesService = moviesService;
        }

        /// <summary>
        /// Translates text from english to spanish.
        /// </summary>
        public async Task TranslateEnToEs()
        {
            var moviesDb = await this.moviesServiceAzure.GetAllAsync();


            foreach (var movie in moviesDb)
            {
                if (!string.IsNullOrEmpty(movie.DescriptionEs)) continue;

                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://google-translate20.p.rapidapi.com/translate"),
                    Headers =
                    {
                        { "x-rapidapi-host", "google-translate20.p.rapidapi.com" },
                        { "x-rapidapi-key", "c585a72cd9mshcf39cef7a10ae83p10abd6jsnf85e6b44d53b" },
                    },
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "text", movie.Description },
                        { "tl", "es" },
                        { "sl", "en" },
                    }),
                };

                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    var translatedText = this.GetTranslatedText(body);
                    movie.DescriptionEs = translatedText;
                    await this.moviesService.Update(movie);
                }
            }
        }

        /// <summary>
        /// Gets the translated text.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>string</returns>
        private string GetTranslatedText(string response)
        {
            var keyword = "translation";
            var keyword2 = "pronunciation";
            var indexFirst = response.IndexOf(keyword) + keyword.Length;
            var indexLast = response.IndexOf(keyword2);
            var translatedText = response.Substring(indexFirst + 3, indexLast - indexFirst - 6);
            return translatedText;
        }
    }
}