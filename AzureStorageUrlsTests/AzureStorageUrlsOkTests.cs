using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Movies.Application.Mappers;
using Movies.Application.Services;
using Movies.Application.ThirdParty;
using Movies.Domain.Entities;
using Movies.Persistence;
using Movies.Persistence.Repositories;
using NUnit.Framework;

namespace AzureStorageUrlsTests
{
    [TestFixture]
    public class AzureStorageUrlsOkTests
    {
        private IServiceCollection services;
        private IServiceProvider serviceProvider;
        private IMoviesServiceAzure moviesService;
        private Mock<IMoviesRepository> moviesRepository;
        private Mock<IMapper> mapper;
        private HttpClient httpClient;

        [SetUp]
        public void Setup()
        {
            this.services = new ServiceCollection();
            this.httpClient = new HttpClient();
            this.mapper = new Mock<IMapper>();
            this.moviesRepository = new Mock<IMoviesRepository>();

            services.AddAutoMapper(AssemblyApplication.Assembly);
            services.AddDbContext<PolluxMoviesDbContext>(options =>
                options.UseSqlServer("Server=localhost;Database=Pollux.Movies;Trusted_Connection=True;"));
            services.AddTransient<IMoviesService, MoviesService>();
            services.AddTransient<IMoviesRepository, MoviesRepository>();
            services.AddTransient<IMoviesListRepository, MoviesListRepository>();
            services.AddTransient<IUserLikesRepository, MoviesLikesRepository>();
            services.AddTransient<IMoviesListService, MoviesListService>();
            services.AddTransient<IMoviesLikesService, MoviesLikesService>();

            this.serviceProvider = this.services.BuildServiceProvider();
            this.moviesService = this.serviceProvider.GetService<IMoviesServiceAzure>();
        }

        [Test]
        public async Task GetUrlCoverImagesOk()
        {
            var moviesList = new List<Movie>();
            this.moviesRepository
                .Setup(p => p.GetManyAsync(p => p.ProcessedByStreamVideo))
                .Returns(Task.FromResult(moviesList));

            moviesList = await this.moviesService.GetAllAsync();

            //this.moviesRepository.Verify(p => p.GetManyAsync(p => p.IsDeleted == false && p.ProcessedByStreamVideo), Times.Once);

            foreach (var movie in moviesList)
            {
                var response = await httpClient.GetAsync(new Uri(movie.UrlCoverImage));
                Assert.True(response.IsSuccessStatusCode);
            }
        }

        [Test]
        public async Task GetUrlPreviewImagesOk()
        {
            var moviesList = new List<Movie>();
            this.moviesRepository
                .Setup(p => p.GetManyAsync(p => p.ProcessedByStreamVideo))
                .Returns(Task.FromResult(moviesList));

            moviesList = await this.moviesService.GetAllAsync();


            foreach (var movie in moviesList)
            {
                var response = await httpClient.GetAsync(new Uri(movie.UrlImage));
                try
                {
                    Assert.True(response.IsSuccessStatusCode);
                }
                catch (Exception e)
                {
                    Console.WriteLine(movie.Name);
                    throw;
                }
            }
        }
    }
}