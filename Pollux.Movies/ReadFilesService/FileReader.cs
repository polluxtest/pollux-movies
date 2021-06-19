using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ReadFilesService
{
    public interface IFileReader
    {
        Task ReadVideosFromDirectory();
        Task ReadImagesFromDirectory();
    }

    public class FileReader : IFileReader
    {
        private const string FilesMoviesPath = @"X:\movies";
        private const string FilesImagesPath = @"W:\images";
        private const string AzureCDNPath = @"https://polluxcdn.azureedge.net";
        private const string AzureImagesContainer = "polluximagescontainer";
        private readonly IFileDbWriter fileDbWriter;


        public FileReader(IFileDbWriter fileDbWriter)
        {
            this.fileDbWriter = fileDbWriter;
        }

        /// <summary>
        /// Reads from folder.
        /// </summary>
        /// <returns></returns>
        public async Task ReadVideosFromDirectory()
        {

            var files = Directory.GetFiles(FilesMoviesPath);

            var filesToUpload = new List<(string, string)>();

            foreach (var file in files)
            {
                var fileName = file.Remove(0, 10);
                var pointIndex = fileName.IndexOf(".");
                var movieName = fileName.Remove(pointIndex, fileName.Length - pointIndex);

                filesToUpload.Add((fileName, movieName));
            }

            await fileDbWriter.WriteToDataBase(filesToUpload);
        }

        /// <summary>
        /// Reads the images from directory.
        /// </summary>
        public async Task ReadImagesFromDirectory()
        {
            var files = Directory.GetFiles(FilesImagesPath);

            var filesToUpload = new List<(string, string)>();

            foreach (var file in files)
            {
                var fileName = file.Remove(0, 10);
                var pointIndex = fileName.IndexOf(".");
                var imageName = fileName.Remove(pointIndex, fileName.Length - pointIndex);
                var azureFilePath = $"{AzureCDNPath}/{AzureImagesContainer}/{fileName}";

                filesToUpload.Add((imageName, azureFilePath));
            }

            await fileDbWriter.WriteImagesToDataBase(filesToUpload);
        }
    }
}
