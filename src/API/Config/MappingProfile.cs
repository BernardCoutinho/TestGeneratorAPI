using AutoMapper;
using TestGeneratorAPI.src.API.Model;
using TestGeneratorAPI.src.API.View.User;
namespace TestGeneratorAPI.src.API.Config
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<User, UserResponse>();
            CreateMap<UserResponse, User>();
        }
    }
}
