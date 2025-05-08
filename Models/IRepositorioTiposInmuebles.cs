
namespace Inmobiliaria_Avgustin.Models
{
    public interface IRepositorioTiposImnuebles : IRepositorio<TiposInmuebles>
    {
        IList<Inquilino> BuscarPorNombre(string nombre);
    }
}