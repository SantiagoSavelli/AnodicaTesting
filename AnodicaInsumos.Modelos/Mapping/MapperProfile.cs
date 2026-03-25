using AnodicaInsumos.Modelos;
using AnodicaInsumos.Modelos.ViewModels;
using AutoMapper;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Perfil, PerfilVM>().ReverseMap()
            .ForMember(dest => dest.PerfilID, opt => opt.Ignore())
            .ForMember(dest => dest.ImagenPerfil, opt => opt.Ignore())
            .ForMember(dest => dest.Tratamientos, opt => opt.Ignore())
            .ForMember(dest => dest.Equivalencias, opt => opt.Ignore())
            .ForMember(dest => dest.UbicacionRef, opt => opt.Ignore());

        CreateMap<Perfil, Perfil>()
            .ForMember(dest => dest.PerfilID, opt => opt.Ignore())
            .ForMember(dest => dest.ImagenPerfil, opt => opt.Ignore())
            .ForMember(dest => dest.Tratamientos, opt => opt.Ignore())
            .ForMember(dest => dest.Equivalencias, opt => opt.Ignore())
            .ForMember(dest => dest.UbicacionRef, opt => opt.Ignore());

        CreateMap<PerfilTratamiento, PerfilTratamientoVM>().ReverseMap();
    }
}