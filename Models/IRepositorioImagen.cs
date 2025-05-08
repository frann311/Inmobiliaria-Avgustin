// Models/IRepositorioImagen.cs
using System.Collections.Generic;

namespace Inmobiliaria_Avgustin.Models
{
    public interface IRepositorioImagen : IRepositorio<Imagen>
    {

        IList<Imagen> BuscarPorInmueble(int inmuebleId);
        void MarcarComoPortada(int id);
        void QuitarPortadaActual(int inmuebleId);
        int CantidadImagenes(int inmuebleId);
    }
}
