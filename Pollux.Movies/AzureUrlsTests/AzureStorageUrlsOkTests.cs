using System;
using System.Net.Http;
using Moq;
using Moq.AutoMock;
using Movies.Application;
using Xunit;

namespace AzureUrlsTests
{
    public class AzureStorageUrlsOkTests
    {
        [Fact]
        public async void ImageCoverTestOk()
        {
            var mocker = new AutoMocker();
            var httpClient = new HttpClient();
            var moviesService = new Mock<IMoviesService>();
            mocker.Use(moviesService);
            var movies = await moviesService.GetAll(false);

            foreach (var movie in movies)
            {
                var response = await httpClient.GetAsync(new Uri(movie.UrlCoverImage));
                Assert.NotEmpty(movie.Name);
                Assert.True(response.IsSuccessStatusCode);
            }
        }
    }
}
