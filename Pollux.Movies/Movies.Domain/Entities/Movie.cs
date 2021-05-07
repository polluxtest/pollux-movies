namespace Movies.Domain
{
    /// <summary>
    /// Movie
    /// </summary>
    public class Movie
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        public string Description { get; set; }

        public string Gender { get; set; }
        
        public string Type { get; set; }
        
        public string Language { get; set; }
        
        public string Year { get; set; }

        public string Url { get; set; }
    }
}