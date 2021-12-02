﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureUploaderTransformerVideos.Constants;
using Movies.Common.Constants.Strings;
using Movies.Common.ExtensionMethods;

namespace ReadFilesService
{
    public interface IFileReader
    {
        Task ReadVideosFromDirectory();
        Task ReadImagesFromDirectory();
        Task ReadSubtitlesFromDirectory();
    }

    public class FileReader : IFileReader
    {
        private const string FilesMoviesPath = @"W:\pollux\newMovies";
        private const string FilesImagesPath = @"W:\pollux\newImages";
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
                var fileName = file.Remove(0, 20);
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
                var fileName = file.Remove(0, 20);
                var pointIndex = fileName.IndexOf(".");
                var imageName = fileName.Remove(pointIndex, fileName.Length - pointIndex);
                var azureFilePath = $"{AzureContainersConstants.AzureCDNPath}/{AzureContainersConstants.AzureImagesContainer}/{fileName}";

                filesToUpload.Add((imageName, azureFilePath));
            }

            await fileDbWriter.WriteImagesToDataBase(filesToUpload);
        }

        public async Task ReadSubtitlesFromDirectory()
        {
            var directories = Directory.GetDirectories(FilesLocalPathsConstants.FilesSubtitlesPath);

            foreach (var directory in directories)
            {
                var filesToUpload = new List<(string, List<string>)>();
                var directorPath = directory.TrimAll().ToLower();
                var directoryName = directorPath.Replace(FilesLocalPathsConstants.FilesSubtitlesPath, string.Empty).Replace("\\", string.Empty);
                var subtitlesList = Directory.GetFiles(directory).Where(p => p.EndsWith(SubtitlesConstants.SRT) && !p.Contains("_")).ToList();
                var subtitlesVTT = await this.SrtToVTTTransform(subtitlesList);
                filesToUpload.Add((directoryName, new List<string>(subtitlesVTT)));
                await fileDbWriter.WriteSubtitlesToDataBase(filesToUpload);

            }

        }

        private List<string> ChangeExtensionToVtt(List<string> subtitles)
        {
            var subtitlesVTT = new List<string>();

            foreach (var subtitle in subtitles)
            {
                var subtitleVTT = Path.ChangeExtension(subtitle, SubtitlesConstants.SRT);
                File.Move(subtitle, subtitleVTT, false);
                subtitlesVTT.Add(subtitleVTT);
            }

            return subtitlesVTT;
        }

        private async Task<List<string>> SrtToVTTTransform(List<string> subtitles)
        {
            var subtitlesVTT = new List<string>();

            foreach (var subtitle in subtitles)
            {
                var stringBuilder = new StringBuilder(SubtitlesConstants.VTTitle);
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                using (StreamReader sr = new StreamReader(subtitle, Encoding.GetEncoding("iso-8859-1")))
                {
                    while (sr.Peek() >= 0)
                    {
                        var line = sr.ReadLine() ?? string.Empty;
                        var updatedLine = line;
                        if (line.Contains(SubtitlesConstants.SubtitleLineMarker))
                        {
                            updatedLine = line.Replace(",", ".");
                        }

                        stringBuilder.AppendLine(updatedLine);
                    }

                    var subtitleVTT = Path.ChangeExtension(subtitle, SubtitlesConstants.VTT);
                    await File.WriteAllTextAsync(subtitleVTT, stringBuilder.ToString(), Encoding.Default);
                    subtitlesVTT.Add(subtitleVTT);
                }
            }

            return subtitlesVTT;

        }
    }
}
