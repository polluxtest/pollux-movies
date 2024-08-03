namespace Movies.Common.Constants.Strings
{

    public static class ApiRoutesConstants
    {
        /// <summary>
        /// The default route.
        /// </summary>
        public const string DefaultRoute = "api/pollux/[controller]";

        /// <summary>
        /// The by director.
        /// </summary>
        public const string ByDirector = "ByDirector";

        /// <summary>
        /// The by genre
        /// </summary>
        public const string ByGenre = "ByGenre";

        /// <summary>
        /// The by language.
        /// </summary>
        public const string ByLanguage = "ByLanguage";


        /// <summary>
        /// My list.
        /// </summary>
        public const string MyList = "GetMyList";

        /// <summary>
        /// My likes.
        /// </summary>
        public const string GetLikesIds = "GetLikesIds";


        /// <summary>
        /// My list.
        /// </summary>
        public const string GetMyListIds = "GetMyListIds";

        /// <summary>
        /// The search.
        /// </summary>
        public const string Search = "Search";

        /// <summary>
        /// The like
        /// </summary>
        public const string Like = "Like";

        /// <summary>
        /// Get Movie. 
        /// </summary>
        public const string Movie = "Movie/{id}";

        /// <summary>
        /// Get Recommended Movies by Pollux.
        /// </summary>
        public const string RecommendedByPollux = "RecommendedByPollux";

        /// <summary>
        /// Get Recommended Movies by Users.
        /// </summary>
        public const string RecommendedByUsers = "RecommendedByUsers";

        /// <summary>
        /// The get movie names
        /// </summary>
        public const string GetMovieSearchOptions = "GetMovieSearchOptions";

        /// <summary>
        /// The featured movies.
        /// </summary>
        public const string FeaturedMovies = "FeaturedMovies";

        /// <summary>
        /// The get IMBD movie ranking
        /// </summary>
        public const string GetImbdMovieRanking = "GetImbdMovieRanking";

        /// <summary>
        /// The get all continue watching
        /// </summary>
        public const string GetAllContinueWatching = "GetAllContinueWatching";

        /// <summary>
        /// The get continue watching
        /// </summary>
        public const string GetContinueWatching = "GetContinueWatching";

        /// <summary>
        /// The get search by genre
        /// </summary>
        public const string GetSearchByGenre = "GetSearchByGenre";

        /// <summary>
        /// Movie Genres Route
        /// </summary>
        public const string GetMoviesGenres = "GetMoviesGenres";
    }
}
