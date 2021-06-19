using System.Collections.Generic;
using Movies.Domain.Entities;

namespace Movies.Application.Models
{
    public class MoviesListModel
    {
        public List<MovieModel> Movies { get; set; }
    }
}
