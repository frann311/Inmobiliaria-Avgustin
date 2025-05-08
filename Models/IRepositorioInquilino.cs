
namespace Inmobiliaria_Avgustin.Models
{
    public interface IRepositorioInquilino : IRepositorio<Inquilino>
    {

        Inquilino ObtenerPorEmail(string email);
        IList<Inquilino> BuscarPorNombre(string nombre);
        IList<Inquilino> BuscarPorDni(string dni);

        IList<Inquilino> obtenerTodosPaginado(int page = 1, int pageSize = 10);
        int CantarInquilinos(); // Devuelve la cantidad de inquilinos en la BD
    }
}