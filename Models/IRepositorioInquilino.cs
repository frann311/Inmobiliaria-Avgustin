
namespace Inmobiliaria_Avgustin.Models
{
    public interface IRepositorioInquilino : IRepositorio<Inquilino>
    {

        Inquilino ObtenerPorEmail(string email);
        IList<Inquilino> BuscarPorNombre(string nombre);
        IList<Inquilino> BuscarPorDni(string dni);
    }
}