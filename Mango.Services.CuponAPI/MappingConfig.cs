using AutoMapper;
using Mango.Services.CuponAPI.Models;
using Mango.Services.CuponAPI.Models.Dto;

namespace Mango.Services.CuponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CuponDto, Cupon>();
                config.CreateMap<Cupon, CuponDto>();
            });

            return mappingConfig;
        }
    }
}
