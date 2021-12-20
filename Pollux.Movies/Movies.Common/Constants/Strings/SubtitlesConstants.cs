namespace Movies.Common.Constants.Strings
{
    public static class SubtitlesConstants
    {
        /// <summary>
        /// The VTT subtitles extension which is used in the video html as opposed to str which for
        /// some reason do not work.
        /// </summary>
        public const string VTT = "vtt";

        /// <summary>
        /// The SRT default common subtitle.
        /// </summary>
        public const string SRT = "srt";

        /// <summary>
        /// The VTT title subtitles header file.
        /// </summary>
        public const string VTTitle = "WEBVTT";

        /// <summary>
        /// The subtitle line marker that indicates the duration of time of line in subtitles.
        /// </summary>
        public const string SubtitleLineMarker = "-->";

    }
}
