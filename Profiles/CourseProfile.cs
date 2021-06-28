using AutoMapper;
using Client.Models;

namespace Client.Profiles
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<CourseFullInfoVM, CourseEditingModel>();
            CreateMap<CourseEditingModel, CourseFullInfoVM>();
        }
    }
}
