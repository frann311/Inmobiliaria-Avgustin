
namespace Inmobiliaria_Avgustin.Models
{
    public interface IRepositorioUsosInmuebles : IRepositorio<UsosInmuebles>
    {
        IList<Inquilino> BuscarPorNombre(string nombre);
    }
}