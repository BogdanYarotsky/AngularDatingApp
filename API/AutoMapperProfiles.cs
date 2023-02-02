using API.DTOs;
using API.Entities;
using API.Extenstions;
using AutoMapper;

namespace API;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, MemberDto>()
            .ForMember(dto => dto.PhotoUrl, url => url.MapFrom(user => user.Photos.First(p => p.IsMain).Url))
            .ForMember(dto => dto.Age, age => age.MapFrom(user => user.DateOfBirth.CalculateAge()));
        CreateMap<Photo, PhotoDto>();

    }
}