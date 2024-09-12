using AutoMapper;
using BookStoreApp.API.Data;
using BookStoreApp.API.DTOs.Author;

namespace BookStoreApp.API.Configurations
{
    public class MapperConfig : Profile {
      public MapperConfig() {
         CreateMap<AuthorReadOnlyDTO, Author>().ReverseMap();
         CreateMap<AuthorCreateDTO, Author>().ReverseMap();
         CreateMap<AuthorUpdateDTO, Author>().ReverseMap();
      }
   }
}