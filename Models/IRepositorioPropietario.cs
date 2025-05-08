
namespace Inmobiliaria_Avgustin.Models
{
    public interface IRepositorioPropietario : IRepositorio<Propietario>
    {

        bool ProbarConexion();
        IList<Propietario> ObtenerTodosPaginado(int page = 1, int pageSize = 10);
        Propietario ObtenerPorEmail(string email);
        IList<Propietario> BuscarPorNombre(string nombre);
        IList<Propietario> BuscarPorDni(string dni);
        int contarPropietarios();
    }
}
