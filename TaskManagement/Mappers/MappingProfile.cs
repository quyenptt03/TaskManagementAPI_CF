using AutoMapper;
using TaskManagement.DTOs;
using TaskManagement.Models;

namespace TaskManagement.Mappers
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Models.Task, TaskDto>().ReverseMap().ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<TaskComment, TaskCommentDto>().ReverseMap();
            CreateMap<Label, LabelDto>().ReverseMap();
            CreateMap<TaskLabel, TaskLabelDto>().ReverseMap();

        }
    }
}
