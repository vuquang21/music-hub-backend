using AutoMapper;
using MusicApp.Domain.Entities;
using MusicApp.Application.Auth.DTOs;
using MusicApp.Application.Tracks.DTOs;
using MusicApp.Application.Albums.DTOs;
using MusicApp.Application.Playlists.DTOs;
using MusicApp.Application.Users.DTOs;
using MusicApp.Application.Home.DTOs;

namespace MusicApp.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role.ToString()));

        CreateMap<Track, TrackDto>()
            .ForMember(d => d.Isrc, opt => opt.MapFrom(s => s.Isrc.Value))
            .ForMember(d => d.DurationSeconds, opt => opt.MapFrom(s => s.Duration.Seconds))
            .ForMember(d => d.DurationFormatted, opt => opt.MapFrom(s => s.Duration.Formatted))
            .ForMember(d => d.ArtistName, opt => opt.MapFrom(s => s.Artist != null ? s.Artist.Name : null))
            .ForMember(d => d.Genres, opt => opt.MapFrom(s => s.Genres.Select(g => g.Slug).ToList()))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

        // Home feed: lightweight track card (album art used as cover image)
        CreateMap<Track, HomeSectionTrackDto>()
            .ForMember(d => d.ArtistName, opt => opt.MapFrom(s => s.Artist != null ? s.Artist.Name : null))
            .ForMember(d => d.CoverImageUrl, opt => opt.MapFrom(s => s.Album != null ? s.Album.CoverImageUrl : null))
            .ForMember(d => d.DurationSeconds, opt => opt.MapFrom(s => s.Duration.Seconds))
            .ForMember(d => d.DurationFormatted, opt => opt.MapFrom(s => s.Duration.Formatted));

        // Home feed: podcast card
        CreateMap<Podcast, HomePodcastDto>();

        CreateMap<Album, AlbumDto>()
            .ForMember(d => d.ArtistName, opt => opt.MapFrom(s => s.Artist != null ? s.Artist.Name : null))
            .ForMember(d => d.TrackCount, opt => opt.MapFrom(s => s.Tracks.Count))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

        CreateMap<Playlist, PlaylistDto>()
            .ForMember(d => d.OwnerName, opt => opt.MapFrom(s => s.Owner != null ? s.Owner.DisplayName : null))
            .ForMember(d => d.TrackCount, opt => opt.MapFrom(s => s.PlaylistTracks.Count))
            .ForMember(d => d.FollowerCount, opt => opt.MapFrom(s => s.Followers.Count));

        CreateMap<Artist, ArtistDto>();
    }
}
