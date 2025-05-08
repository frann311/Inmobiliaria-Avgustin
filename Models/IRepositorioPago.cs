
namespace Inmobiliaria_Avgustin.Models
{
    public interface IRepositorioPago : IRepositorio<Pago>
    {
        IList<Pago> ObtenerPagosPorContrato(int contratoId);
        IList<Pago> ObtenerPagosPorInquilino(int inquilinoId);
        IList<Pago> ObtenerPagosPorPropietario(int propietarioId);
        int AnularPagosFueraDeRango(int contratoId, DateTime Rescision);
        int ObtenerUltimoNumeroPago(int contratoId);
        int Anular(int id, int usuarioId);
        int Pagar(Pago pago, DateTime fechaPago, int usuarioId);
    }
}

