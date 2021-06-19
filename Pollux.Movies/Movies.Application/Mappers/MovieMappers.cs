using System.Linq;
using AutoMapper;
using Movies.Application.Models;
using Movies.Domain.Entities;

namespace Movies.Application.Mappers
{
    public class MovieMappers : Profile
    {
        public MovieMappers()
        {
            this.CreateMap<Movie, MovieModel>();

            this.CreateMap<MoviesListModel, MoviesByDirectorModel>()
                .ForMember(p => p.DirectorName, i => i.MapFrom(p => p.Movies.FirstOrDefault().Director.Name))
                .ForMember(p => p.DirectorId, i => i.MapFrom(p => p.Movies.FirstOrDefault().Director.Id));
        }
    }
}
