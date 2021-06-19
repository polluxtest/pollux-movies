using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Movies.Application;
using Movies.Domain.Entities;
using Movies.Common.ExtensionMethods;

namespace ReadFilesService
{
    public interface IFileDbWriter
    {

        Task WriteToDataBase(List<(string, string)> files);
        Task WriteImagesToDataBase(List<(string, string)> files);
    }

    public class FileDBWriter : IFileDbWriter
    {
        private readonly IMoviesService _moviesesService;

        public FileDBWriter(IMoviesService moviesesService)
        {
            this._moviesesService = moviesesService;
        }

        public Task WriteToDataBase(List<(string, string)> files)
        {
            foreach (var file in files)
            {
                var movie = new Movie()
                {
                    Description = string.Empty,
                    DirectorId = 1,
                    FileName = file.Item1,
                    Name = file.Item2,
                    Gender = string.Empty,
                    Language = string.Empty,
                    Year = string.Empty,
                    Type = string.Empty,
                    Likes = 0,
                    UrlImage = string.Empty,
                    UrlVideo = string.Empty,
                    IsDeleted = false,
                    ProcessedByAzureJob = false
                };

                this._moviesesService.Add(movie);

            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Writes the images to data base.
        /// </summary>
        /// <param name="files">The files. (fileName, azureCDNPath)</param>
        /// <returns></returns>
        public async Task WriteImagesToDataBase(List<(string, string)> files)
        {
            var movies = await _moviesesService.GetAll(true);

            foreach (var movie in movies)
            {
                if (!movie.UrlImage.Equals(string.Empty) && movie.UrlImage != null)
                {
                    continue;
                }

                var movieNameTrimmed = movie.Name.TrimAll();
                var imagePath = files.SingleOrDefault(p =>
                    p.Item1.Equals(movieNameTrimmed, StringComparison.OrdinalIgnoreCase)).Item2;

                movie.UrlImage = imagePath ?? string.Empty;
                await _moviesesService.UpdateMovie(movie);
            }
        }
    }
}
