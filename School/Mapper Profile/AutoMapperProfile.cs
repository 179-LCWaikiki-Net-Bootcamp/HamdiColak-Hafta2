using AutoMapper;
using School.Entities;
using School.Entity_DTOs;

namespace School.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<StudentDTO, Student>()
                // The variable names in DTO can differ from the Entity. We should notify that to the mapper profile as shown below
                .ForMember(dest => dest.FatherName, opt => opt.MapFrom(src => src.DadName))
                //.ForMember(dest => dest.Adress, opt => opt.MapFrom(src => src.AddresDTO))
                //.ForMember(dest => dest.Grade, opt => opt.MapFrom(src => src.GradeDTO))
                // We can set entity variables according to the DTO variables' values
                .ForMember(dest => dest.IsAdult, opt => opt.MapFrom(src => src.Age > 18 ? true : false));

            // Reverse mapping for the 'Get' Method
            CreateMap<Student, StudentDTO>()
                // The variable names in DTO can differ from the Entity. We should notify that to the mapper profile as shown below
                .ForMember(dest => dest.DadName, opt => opt.MapFrom(src => src.FatherName));
            //.ForMember(dest => dest.AddresDTO, opt => opt.MapFrom(src => src.Adress))
            //.ForMember(dest => dest.GradeDTO, opt => opt.MapFrom(src => src.Grade));

            CreateMap<GradeDTO, Grade>();
            CreateMap<Grade, GradeDTO>();

            CreateMap<AddressDTO, Address>();
            CreateMap<Address, AddressDTO>();

           
        }
    }
}
