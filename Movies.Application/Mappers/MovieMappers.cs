using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ObjectiveC;
using AutoMapper;
using Movies.Application.Models;
using Movies.Domain.Entities;
using Movies.Persistence.QueryResults;
using Newtonsoft.Json;

namespace Movies.Application.Mappers
{
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
                .ReverseMap();

            this.CreateMap<MovieUserRequest, MoviesLists>();

            this.CreateMap<MovieWatching, MovieWatchingModel>()
                .ForMember(
                    dest => dest.Movie,
                    opt => opt.MapFrom(p => p.Movie))
                .ForPath(
                    dest => dest.Movie.Duration,
                    opt => opt.MapFrom(p => p.Duration))
                .ForPath(
                    dest => dest.Movie.ElapsedTime,
                    opt => opt.MapFrom(p => p.ElapsedTime))
                .ForPath(
                    dest => dest.Movie.RemainingTime,
                    opt => opt.MapFrom(p => p.RemainingTime));

            this.CreateMap<MovieWatching, MovieModel>()
                .ForMember(p => p.Name, opt => opt.MapFrom(p => p.Movie.Name))
                .ForMember(p => p.Description, opt => opt.MapFrom(p => p.Movie.Description))
                .ForMember(p => p.DescriptionEs, opt => opt.MapFrom(p => p.Movie.DescriptionEs))
                .ForMember(p => p.Imbd, opt => opt.MapFrom(p => p.Movie.Imbd))
                .ForMember(p => p.DirectorName, opt => opt.MapFrom(p => p.Movie.Director.Name))
                .ForMember(p => p.Language, opt => opt.MapFrom(p => p.Movie.Language))
                .ForMember(p => p.UrlImage, opt => opt.MapFrom(p => p.Movie.UrlImage))
                .ForMember(p => p.Year, opt => opt.MapFrom(p => p.Movie.Year))
                .ForMember(p => p.Id, opt => opt.MapFrom(p => p.Movie.Id));

            this.CreateMap<MovieGenres, MovieGenreModel>();

            this.CreateMap<MovieUserRequest, MoviesLikes>();

            this.CreateMap<MovieFeatured, MovieFeaturedModel>().ForMember(
                dest => dest.Movie,
                opt => opt.MapFrom(p => p.Movie));

            this.CreateMap<MoviesQueryResult, MovieModel>()
                .ForMember(p => p.Name, opt => opt.MapFrom(p => p.Movie.Name))
                .ForMember(p => p.Description, opt => opt.MapFrom(p => p.Movie.Description))
                .ForMember(p => p.DescriptionEs, opt => opt.MapFrom(p => p.Movie.DescriptionEs))
                .ForMember(p => p.Imbd, opt => opt.MapFrom(p => p.Movie.Imbd))
                .ForMember(p => p.DirectorName, opt => opt.MapFrom(p => p.Movie.Director.Name))
                .ForMember(p => p.Language, opt => opt.MapFrom(p => p.Movie.Language))
                .ForMember(p => p.UrlImage, opt => opt.MapFrom(p => p.Movie.UrlImage))
                .ForMember(p => p.Year, opt => opt.MapFrom(p => p.Movie.Year))
                .ForMember(p => p.Id, opt => opt.MapFrom(p => p.Movie.Id));


            this.CreateMap<MoviesQueryResult, MovieWatchingModel>();

            this.CreateMap<MovieModel, MovieWatchingModel>()
                .ForMember(p => p.Movie, opt => opt.MapFrom(p => p));
        }
    }
}