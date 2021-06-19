using System.Collections.Generic;

namespace Movies.Application.Models
{
    public class MoviesByDirectorModel
    {
        public int DirectorId { get; set; }

        public string DirectorName { get; set; }

        private IList<MovieModel> Movies { get; set; }
    }
}
