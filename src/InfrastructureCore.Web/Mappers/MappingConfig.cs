using AutoMapper;
using InfrastructureCore.Models.Identity;
using InfrastructureCore.Models.Menu;
using System.Collections.Generic;

namespace InfrastructureCore.Web.Mappers
{
    public class MappingConfig: Profile
    {
        public MappingConfig()
        {
           // CreateMap<ChunkMetadata, SYFileUpload>();
            CreateMap<SYUser, SYLoggedUser>(); 
            CreateMap<SYUser, SYUserAccessMenus>().ForMember(dest => dest.USER_ID, atc => atc.MapFrom(src=>src.UserID))
                .ForMember(dest => dest.USER_NAME, atc => atc.MapFrom(src => src.UserName))
                .ForMember(dest => dest.USER_CODE, atc => atc.MapFrom(src => src.UserCode));             
        }
    }
}
