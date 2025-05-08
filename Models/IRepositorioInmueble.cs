using Inmobiliaria_Avgustin.Models;

public interface IRepositorioInmueble : IRepositorio<Inmueble>
{

    IList<Inmueble> ObtenerTodosFilter(bool? disponible, int? propietarioId, DateTime? fechaDesde, DateTime? fechaHasta, int? TipoId, int? UsoId, int page = 1, int pageSize = 10);
    IEnumerable<Inmueble> ObtenerPorPropietario(int idPropietario);
    IEnumerable<Inmueble> ObtenerPorTipo(string tipo);
    IEnumerable<Inmueble> ObtenerPorPrecio(decimal precioMinimo, decimal precioMaximo);
    IEnumerable<Inmueble> ObtenerPorUbicacion(string ubicacion);
    IEnumerable<Inmueble> ObtenerPorFecha(DateTime fechaDesde, DateTime fechaHasta);
    IList<Inmueble> BuscarPorNombre(string nombre);

    int ContarInmueblesFilter(bool? disponible, int? propietarioId, DateTime? fechaDesde, DateTime? fechaHasta, int? TipoId, int? UsoId);
}
