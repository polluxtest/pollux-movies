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

            this.CreateMap<MoviesListModel, MoviesByCategoryModel>()
                .ForMember(p => p.Title, i => i.MapFrom(p => p.Movies.FirstOrDefault().Director.Name));
        }
    }
}
