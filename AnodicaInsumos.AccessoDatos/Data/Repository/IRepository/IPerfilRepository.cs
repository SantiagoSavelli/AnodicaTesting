using AnodicaInsumos.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodicaInsumos.AccessoDatos.Data.Repository.IRepository
{
    public interface IPerfilRepository : IRepository<Perfil, int> 
    {
        void Update(Perfil perfil);
    }
}
