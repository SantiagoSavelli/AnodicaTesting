using AnodicaInsumos.Modelos;
using AnodicaInsumos.Modelos.ViewModels;
using AutoMapper;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Perfil, PerfilVM>().ReverseMap();
        CreateMap<PerfilTratamiento, PerfilTratamientoVM>().ReverseMap();
    }
}