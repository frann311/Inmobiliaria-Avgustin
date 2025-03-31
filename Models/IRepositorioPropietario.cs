
namespace Inmobiliaria_Avgustin.Models
{
    public interface IRepositorioPropietario : IRepositorio<Propietario>
    {

        bool ProbarConexion();
        Propietario ObtenerPorEmail(string email);
        IList<Propietario> BuscarPorNombre(string nombre);
        IList<Propietario> BuscarPorDni(string dni);
    }
}
