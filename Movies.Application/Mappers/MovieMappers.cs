using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Movies.Application.Models;
using Movies.Domain.Entities;
using Newtonsoft.Json;

namespace Movies.Application.Mappers
{
    public class MovieMappers : Profile
    {
        public MovieMappers()
        {
            this.CreateMap<Movie, MovieModel>();

            this.CreateMap<Movie, MovieInfoModel>()
                .ForMember(
                    dest => dest.Subtitles,
                    opt =>
                        opt.MapFrom(p => JsonConvert.DeserializeObject<List<string>>(p.Subtitles)))
                .ForMember(dest => dest.DirectorName, opt => opt.MapFrom(p => p.Director.Name));

            this.CreateMap<AddRemoveUserMovieModel, UserMovies>();

            this.CreateMap<AddRemoveUserMovieModel, UserLikes>();

            this.CreateMap<MovieFeatured, MovieFeaturedModel>()
                .ForMember(dest => dest.DirectorName, opt => opt.MapFrom(p => p.Movie.Director.Name))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(p => p.Movie.Name));
        }
    }
}
