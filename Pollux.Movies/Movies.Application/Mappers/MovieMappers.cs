﻿using System.Linq;
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

            this.CreateMap<Movie, MovieInfoModel>();

            this.CreateMap<AddRemoveUserMovieModel, UserMovies>();

            this.CreateMap<AddRemoveUserMovieModel, UserLikes>();
        }
    }
}
