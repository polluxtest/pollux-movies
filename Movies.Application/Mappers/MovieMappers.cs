namespace Movies.Application.Mappers
{
    using System.Collections.Generic;

    using AutoMapper;

    using Movies.Common.Models;
    using Movies.Domain.Entities;
    using Movies.Persistence.Queries;

    using Newtonsoft.Json;

    public class MovieMappers : Profile
    {
        public MovieMappers()
        {
            this.CreateMap<Movie, MovieModel>()
                .ForMember(
                    dest => dest.Subtitles,
                    opt =>
                        opt.MapFrom(p => JsonConvert.DeserializeObject<List<string>>(p.Subtitles)))
                .ForMember(dest => dest.DirectorName, opt => opt.MapFrom(p => p.Director.Name))
                .ForMember(dest => dest.Genre, opt => opt.MapFrom(p => p.Genre.Name))
                .ReverseMap();


            this.CreateMap<MovieWatching, MovieWatchingModel>()
                .ForMember(
                    dest => dest.Movie,
                    opt => opt.MapFrom(p => p.Movie))
                .ForPath(
                    dest => dest.Duration,
                    opt => opt.MapFrom(p => p.Duration))
                .ForPath(
                    dest => dest.ElapsedTime,
                    opt => opt.MapFrom(p => p.ElapsedTime))
                .ForPath(
                    dest => dest.RemainingTime,
                    opt => opt.MapFrom(p => p.RemainingTime));

            this.CreateMap<MovieModel, MovieWatchingModel>()
                .ForMember(p => p.Movie, opt => opt.MapFrom(p => p));

            this.CreateMap<MovieWatching, MovieModel>()
                .IncludeMembers(p => p.Movie);

            this.CreateMap<MoviesQuery, MovieModel>()
                .IncludeMembers(p => p.Movie)
                .ForMember(p => p.DirectorName, opt => opt.MapFrom(p => p.Director.Name))
                .ForMember(p => p.Genre, opt => opt.MapFrom(p => p.Genre));


            this.CreateMap<MoviesQuery, MovieWatchingModel>()
                .ForMember(p => p.Movie, opt => opt.MapFrom(p => p.Movie))
                .ForPath(p => p.Movie.Genre, opt => opt.MapFrom(p => p.Genre))
                .ForPath(p => p.Movie.CategoryGenres, opt => opt.MapFrom(p => p.CategoryGenres));
            this.CreateMap<MovieFeaturedQuery, MovieFeaturedModel>()
                .ForMember(p => p.Movie, opt => opt.MapFrom(p => p.Movie))
                .ForPath(p => p.Movie.Genre, opt => opt.MapFrom(p => p.Genre))
                .ForPath(p => p.Movie.CategoryGenres, opt => opt.MapFrom(p => p.CategoryGenres));
        }
    }
}