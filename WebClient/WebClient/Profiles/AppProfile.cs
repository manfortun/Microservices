using AutoMapper;
using WebClient.DTOs;
using WebClient.Enums;
using WebClient.Models;

namespace WebClient.Profiles;

public class AppProfile : Profile
{
    public AppProfile()
    {
        // Registration
        CreateMap<RegisterCredentials, LogInCredentials>();
        CreateMap<RegisterCredentials, RegisterCredentialsDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => ((Role)src.Role).ToString()));
    }
}
