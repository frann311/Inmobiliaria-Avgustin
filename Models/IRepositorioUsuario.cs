using System.Collections.Generic;

namespace Inmobiliaria_Avgustin.Models
{
    public interface IRepositorioUsuario : IRepositorio<Usuario>
    {
        Usuario ObtenerPorEmail(string email);

    }
}
