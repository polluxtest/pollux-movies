namespace Movies.Application.Models.Requests
{
    public class SearchByGenreRequest
    {
        public string UserId { get; set; }
        public string SortBy { get; set; }
        public string Genre { get; set; }
    }
}
