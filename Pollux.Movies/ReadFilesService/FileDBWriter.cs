using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Movies.Application;
using Movies.Domain.Entities;
using Movies.Common.ExtensionMethods;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using AzureUploaderTransformerVideos;
using AzureUploaderTransformerVideos.Constants;
using Movies.Common.Constants.Strings;

namespace ReadFilesService
{
    public interface IFileDbWriter
    {
        Task WriteToDataBase(List<(string, string)> files);
        Task WriteImagesToDataBase(List<(string, string)> files);
        Task WriteSubtitlesToDataBase(List<(string, List<string>)> files);
    }

    public class FileDBWriter : IFileDbWriter
    {
        private readonly IMoviesService _moviesService;
        private readonly IAzureBlobsService blobService;

        public FileDBWriter(IMoviesService moviesService, IAzureBlobsService blobService)
        {
            this._moviesService = moviesService;
            this.blobService = blobService;
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

                this._moviesService.Add(movie);

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
            var movies = await _moviesService.GetAllImages(true);

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
                await _moviesService.UpdateMovie(movie);
            }
        }

        public async Task WriteSubtitlesToDataBase(List<(string, List<string>)> files)
        {
            var movies = await _moviesService.GetAll(true);

            foreach (var file in files)
            {
                var movieDb = movies.SingleOrDefault(p => p.Name.TrimAll().ToLower().Equals(file.Item1));

                if (movieDb == null)
                {
                    throw new ArgumentException($"movie not found with {file.Item1}");
                }

                List<string> subtitlesListDeserialized = JsonConvert.DeserializeObject<List<string>>(movieDb.Subtitles ?? string.Empty);

                if (subtitlesListDeserialized == null) subtitlesListDeserialized = new List<string>();

                foreach (var subtitle in file.Item2)
                {
                    var subtitleName = subtitle.Replace(FilesLocalPathsConstants.FilesSubtitlesPath, string.Empty).Replace("\\", "-").Remove(0, 1);

                    if (await this.blobService.CheckBlobFileExistsAsync(AzureContainersConstants.AzureSubtitlesContainer, subtitleName))
                    {
                        continue;
                    }

                    var azureSubtitleUrl = $"{AzureContainersConstants.AzureCDNPath}/{AzureContainersConstants.AzureSubtitlesContainer}/{subtitleName}";
                    var subtitleUri = await this.blobService.UploadBlobFileAsync(AzureContainersConstants.AzureSubtitlesContainer,
                        subtitleName, subtitle);

                    if (string.IsNullOrEmpty(subtitleUri))
                    {
                        throw new ArgumentException($"movie not found with {subtitleUri}");
                    }

                    subtitlesListDeserialized.Add(azureSubtitleUrl);

                }

                movieDb.Subtitles = JsonConvert.SerializeObject(subtitlesListDeserialized);
                await _moviesService.UpdateMovie(movieDb);

            }
        }
    }
}
