using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.DTOs;
using MovieAPI.Entity;
using NetTopologySuite.Geometries;

namespace MovieAPI.Utils;

public class AutoMapperProfile: Profile
{
    public AutoMapperProfile(GeometryFactory geometryFactory)
    {
        CreateMap<Genre, GenresDTO>().ReverseMap();
        CreateMap<GenreCreateDTO, Genre>();
        CreateMap<Actor, ActorDTO>().ReverseMap();
        CreateMap<ActorCreateDTO, Actor>().ForMember(x => x.picture, opt => opt.Ignore());
        CreateMap<TheatherCreateDTO, Theather>()
            .ForMember(x => x.location, x => 
                x.MapFrom(dto => geometryFactory.CreatePoint(new Coordinate(dto.longitude, dto.latitude))));
        CreateMap<Theather,TheatherDTO>()
            .ForMember(x => x.latitude, dto => dto.MapFrom(fiel => fiel.location.Y))
            .ForMember(x => x.longitude, dto => dto.MapFrom(field => field.location.X));

        CreateMap<MovieCreateDTO, Movie>()
            .ForMember(x => x.poster, options => options.Ignore())
            .ForMember(x => x.MoviesGenres, options => options.MapFrom(MappingMoviesGenrers))
            .ForMember(x => x.MoviesTheathers, options => options.MapFrom(MappingMoviesTheathers))
            .ForMember(x => x.MoviesActors, options => options.MapFrom(MappingMoviesActors));
        
        CreateMap<Movie, MovieDTO>()
            .ForMember(x => x.genres, options => options.MapFrom(MappingMoviesGenrers))
            .ForMember(x =>  x.actors, options => options.MapFrom(MappingMoviesActors))
            .ForMember(x => x.theather, options => options.MapFrom(MappingMoviesTheathers));

        CreateMap<IdentityUser, UserDTO>();
    }

    private List<TheatherDTO> MappingMoviesTheathers(Movie movies, MovieDTO movieDTO)
    {
        var result = new List<TheatherDTO>();
        if (movies.MoviesTheathers != null)
        {
            foreach (var theatherMovie in movies.MoviesTheathers)
            {
                result.Add(new TheatherDTO()
                {
                    id = theatherMovie.theaterId,
                    name = theatherMovie.theater.name,
                    latitude = theatherMovie.theater.location.Y,
                    longitude = theatherMovie.theater.location.X
                });
            }
        }

        return result;
    }
    private List<MovieActorDTO> MappingMoviesActors(Movie movies, MovieDTO movieDTO)
    {
        var result = new List<MovieActorDTO>();
        if (movies.MoviesActors != null)
        {
            foreach (var actorMovie in movies.MoviesActors)
            {
                result.Add(new MovieActorDTO()
                {
                    id = actorMovie.actorId,
                    name = actorMovie.actor.name,
                    picture = actorMovie.actor.picture,
                    order = actorMovie.order,
                    character = actorMovie.character
                });
            }
        }

        return result;
    }
    
    private List<GenresDTO> MappingMoviesGenrers(Movie movies, MovieDTO movieDTO)
    {
        var result = new List<GenresDTO>();
        if (movies.MoviesGenres != null)
        {
            foreach (var genre in movies.MoviesGenres)
            {
                result.Add(new GenresDTO(){ Id = genre.genreId, Name = genre.genre.name});
            }
        }

        return result;
    }
    private List<MoviesGenres> MappingMoviesGenrers(MovieCreateDTO movieCreateDTO, Movie movie)
    {
        var result = new List<MoviesGenres>();

        if (movieCreateDTO.genreIds == null)
        {
            return result;
        }

        foreach (var id in movieCreateDTO.genreIds)
        {
            result.Add(new MoviesGenres() {genreId = id});
        }

        return result;
    }
    
    private List<MoviesTheathers> MappingMoviesTheathers(MovieCreateDTO movieCreateDTO, Movie movie)
    {
        var result = new List<MoviesTheathers>();

        if (movieCreateDTO.theatherIds == null)
        {
            return result;
        }

        foreach (var id in movieCreateDTO.theatherIds)
        {
            result.Add(new MoviesTheathers() {theaterId = id});
        }

        return result;
    }
    
    private List<MoviesActors> MappingMoviesActors(MovieCreateDTO movieCreateDTO, Movie movie)
    {
        var result = new List<MoviesActors>();

        if (movieCreateDTO.actors == null)
        {
            return result;
        }

        foreach (var actor in movieCreateDTO.actors)
        {
            result.Add(new MoviesActors() {actorId = actor.id, character = actor.character});
        }

        return result;
    }
}