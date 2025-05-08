using Azure;
using Inmobiliaria_Avgustin.Models;


public interface IRepositorioContrato : IRepositorio<Contrato>
{


    int Alta(Contrato p, int usuarioId);
    int Modificacion(Contrato p, int usuarioId);
    IList<Contrato> ObtenerContratosFilter(DateTime? fechaDesde, DateTime? fechaHasta, int? diasVencimiento, int? idInmuebleSeleccionado, int page = 1, int pageSize = 10);
    IList<Contrato> ObtenerContratosPorInquilino(int q);
    IList<Contrato> ObtenerContratosPorInmueble(int q);

    int ContarContratosFilter(DateTime? fechaDesde, DateTime? fechaHasta, int? diasVencimiento, int? idInmuebleSeleccionado);



}
