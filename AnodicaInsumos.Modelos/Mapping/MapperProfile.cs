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
            .ForMember(dest => dest.Equivalencias, opt => opt.Ignore())
            .ForMember(dest => dest.UbicacionRef, opt => opt.MapFrom(src => (short?)null));

        CreateMap<PerfilVM, Perfil>()
            .ForMember(dest => dest.PerfilID, opt => opt.Ignore())
            .ForMember(dest => dest.ImagenPerfil, opt => opt.Ignore())
            .ForMember(dest => dest.Equivalencias, opt => opt.Ignore())
            .ForMember(dest => dest.UbicacionRef, opt => opt.MapFrom(src => (short?)null))
            .AfterMap((src, dest, ctx) =>
            {
                ctx.Mapper.Map(src.Perfil, dest);

                foreach (var tratamiento in src.PerfilTratamientos)
                {
                    var existente = dest.Tratamientos.FirstOrDefault(x => x.TratamientoRef == tratamiento.TratamientoRef);

                    if (tratamiento.UbicacionRef == null && tratamiento.CantMinimaTirasStock == 0 && existente != null)
                    {
                        dest.Tratamientos.Remove(existente);
                        continue;
                    }
                    else if (existente != null)
                    {
                        existente.UbicacionRef = tratamiento.UbicacionRef;
                        existente.CantMinimaTirasStock = tratamiento.CantMinimaTirasStock;
                    }
                    else if (existente == null && (tratamiento.UbicacionRef == null && tratamiento.CantMinimaTirasStock == 0))
                    {
                        continue;
                    }
                    else
                    {
                        dest.Tratamientos.Add(new PerfilTratamiento
                        {
                            PerfilRef = dest.PerfilID,
                            TratamientoRef = tratamiento.TratamientoRef,
                            UbicacionRef = tratamiento.UbicacionRef,
                            CantMinimaTirasStock = tratamiento.CantMinimaTirasStock
                        });
                    }
                }
            });

        CreateMap<PerfilTratamiento, PerfilTratamientoVM>().ReverseMap();
    }
}