using AuthService.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Profiles;

public class AppProfile : Profile
{
    public AppProfile()
    {
        CreateMap<RegisterCredentials, IdentityUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
    }
}
