using AutoMapper;
using TestGeneratorAPI.src.API.Model;
using TestGeneratorAPI.src.API.View;
using TestGeneratorAPI.src.API.View.Task;
using TestGeneratorAPI.src.API.View.User;
namespace TestGeneratorAPI.src.API.Config
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<TaskItemRequest, TaskItem>();
            CreateMap<TaskItem, TaskItemRequest>();
            CreateMap<TaskItem, TaskItemResponse>();
            CreateMap<TaskItemResponse, TaskItem>();
            CreateMap<User, UserResponse>();
            CreateMap<UserResponse, User>();
        }
    }
}
