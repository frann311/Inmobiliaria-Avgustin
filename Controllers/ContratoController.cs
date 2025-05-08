using System.Security.Claims;
using Inmobiliaria_Avgustin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class ContratoController : Controller
{
    private readonly IRepositorioContrato _repositorioContrato;
    private readonly IRepositorioInquilino _repositorioInquilino;
    private readonly IRepositorioInmueble _repositorioInmueble;
    private readonly IRepositorioPago _repositorioPago;


    public ContratoController(IRepositorioContrato repositorioContrato, IRepositorioInquilino repositorioInquilino, IRepositorioInmueble repositorioInmueble, IRepositorioPago repositorioPago)
    {
        _repositorioContrato = repositorioContrato;
        _repositorioInquilino = repositorioInquilino;
        _repositorioInmueble = repositorioInmueble;
        _repositorioPago = repositorioPago;

    }



    public IActionResult Index(DateTime? fechaDesde, DateTime? fechaHasta, int? diasVencimiento, int? InmuebleId, int page = 1)
    {
        const int pageSize = 10;

        ViewBag.InmuebleSeleccionado = null;
        int idInmuebleSeleccionado = 0;

        // 1) Obtengo el inmueble seleccionado (si existe)
        Inmueble inmuebleSeleccionado = null;
        if (InmuebleId.HasValue && InmuebleId.Value != 0)
        {
            inmuebleSeleccionado = _repositorioInmueble.ObtenerPorId(InmuebleId.Value);
            idInmuebleSeleccionado = inmuebleSeleccionado.Id;
        }
        ViewBag.InmuebleSeleccionado = inmuebleSeleccionado;


        var contratos = _repositorioContrato.ObtenerContratosFilter(fechaDesde, fechaHasta, diasVencimiento, idInmuebleSeleccionado, page, pageSize);
        ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
        ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");
        ViewBag.DiasVencimiento = diasVencimiento;
        ViewBag.Inmuebles = _repositorioInmueble.ObtenerPorId(InmuebleId ?? 0);
        ViewBag.Page = page;

        int totalRegistros = _repositorioContrato.ContarContratosFilter(fechaDesde, fechaHasta, diasVencimiento, idInmuebleSeleccionado);
        int totalPaginas = (int)Math.Ceiling((double)totalRegistros / pageSize);
        ViewBag.TotalPaginas = totalPaginas;

        return View(contratos);
    }

    [HttpGet]
    public IActionResult Create(int id)
    {
        var inmueble = _repositorioInmueble.ObtenerPorId(id);
        if (inmueble == null)
        {
            return NotFound(); // Si no se encuentra el inmueble, devuelve un error 404
        }
        ViewBag.Inquilinos = _repositorioInquilino.ObtenerTodos() ?? new List<Inquilino>();
        var contrato = new Contrato();
        contrato.Id_Inmueble = id;
        contrato.Fecha_Inicio = DateTime.Today;
        contrato.Fecha_Fin = DateTime.Today.AddDays(1);
        contrato.Monto = (decimal)inmueble.Precio; // Asignar el precio del inmueble al monto del contrato

        var res = _repositorioContrato.ObtenerContratosPorInmueble(id);
        if (res != null && res.Count > 0)
        {
            var contratosVigentes = res
                .Where(c =>
                    c.Fecha_Fin.Date > DateTime.Today &&
                    (c.Fecha_Rescision == null || c.Fecha_Rescision.Value.Date > DateTime.Today)
                )
                .OrderBy(c => c.Fecha_Inicio)
                .ToList();

            ViewBag.ContratosVigentes = contratosVigentes;
        }
        else
        {
            ViewBag.ContratosVigentes = null; // No hay contratos vigentes
        }
        return View(contrato);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Contrato contrato)
    {

        ModelState.Remove("Inquilino"); // <-- Ignoramos esa validación
        ModelState.Remove("Inmueble"); // <-- Ignoramos esa validación
        ModelState.Remove("Creado_En"); // <-- Ignoramos esa validación
        ModelState.Remove("Actualizado_En"); // <-- Ignoramos esa validación
        ModelState.Remove("Fecha_Rescision"); // <-- Ignoramos esa validación
        ModelState.Remove("Multa"); // <-- Ignoramos esa validación

        ModelState.Remove("UsuarioCreador"); // <-- Ignoramos esa validación
        ModelState.Remove("UsuarioFinalizador"); // <-- Ignoramos esa validación
        ModelState.Remove("UsuarioCreadorId"); // <-- Ignoramos esa validación
        ModelState.Remove("UsuarioFinalizadorId"); // <-- Ignoramos esa validación


        Console.WriteLine("Datos recibidos en Create CONTRATO:");
        Console.WriteLine($"Id_Inmueble: {contrato.Id_Inmueble}");
        Console.WriteLine($"Id_Inquilino: {contrato.Id_Inquilino}");
        Console.WriteLine($"Monto: {contrato.Monto}");
        Console.WriteLine($"Fecha_Inicio: {contrato.Fecha_Inicio}");
        Console.WriteLine($"Fecha_Fin: {contrato.Fecha_Fin}");


        var contratos = _repositorioContrato.ObtenerContratosPorInmueble(contrato.Id_Inmueble);
        var inmueble = _repositorioInmueble.ObtenerPorId(contrato.Id_Inmueble);
        if (inmueble == null)
        {
            return NotFound(); // Si no se encuentra el inmueble, devuelve un error 404
        }
        if (inmueble.Disponible == false)
        {
            return NotFound(); // Si no se encuentra el inmueble, devuelve un error 404
        }

        bool haySolapamiento = contratos.Any(c =>
        {
            DateTime fechaFinContratoExistente = c.Fecha_Rescision ?? c.Fecha_Fin;
            return (contrato.Fecha_Inicio <= fechaFinContratoExistente && contrato.Fecha_Fin >= c.Fecha_Inicio);
        });


        if (contrato.Fecha_Inicio.Date < DateTime.Today)
        {
            ModelState.AddModelError("Fecha_Inicio", "La fecha de inicio no puede ser menor a hoy.");
        }
        if (contrato.Fecha_Fin.Date < contrato.Fecha_Inicio.Date)
        {
            ModelState.AddModelError("Fecha_Fin", "La fecha de fin no puede ser menor que la fecha de inicio.");
        }
        if (haySolapamiento)
        {
            ModelState.AddModelError("Fecha_Inicio", "Las fechas del contrato se solapan con otro ya existente.");
            ModelState.AddModelError("Fecha_Fin", "Las fechas del contrato se solapan con otro ya existente.");
        }

        if (ModelState.IsValid) // Verifica si los datos cumplen con las validaciones
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);


            int contratoId = _repositorioContrato.Alta(contrato, userId);
            contrato.Id = contratoId;


            var meses = ((contrato.Fecha_Fin.Year - contrato.Fecha_Inicio.Year) * 12)
                       + contrato.Fecha_Fin.Month - contrato.Fecha_Inicio.Month;
            for (int i = 0; i < meses; i++)
            {
                var pago = new Pago
                {
                    NumeroPago = i + 1,
                    Fecha_Vencimiento = contrato.Fecha_Inicio.AddMonths(i).AddDays(1),
                    Importe = contrato.Monto,
                    ContratoId = contrato.Id,
                    Anulado = false,
                    Concepto = "Alquiler Mensual"
                };
                // Console.WriteLine("INFORMACION DE PAGOS");
                // Console.WriteLine($"NumeroPago: {pago.NumeroPago}");
                // Console.WriteLine($"Fecha_Vencimiento: {pago.Fecha_Vencimiento}");
                _repositorioPago.Alta(pago); // Guarda el pago en la BD
                // Console.WriteLine($"Mes {i + 1}: {contrato.Fecha_Inicio.AddMonths(i).ToString("MMMM yyyy")}");
            }
            TempData["Success"] = "Contrato creado correctamente.";
            return RedirectToAction("Index"); // Redirige a la lista
        }
        var res = _repositorioContrato.ObtenerContratosPorInmueble(contrato.Id);
        if (res != null && res.Count > 0)
        {
            var contratosVigentes = res
                   .Where(c =>
                       c.Fecha_Fin.Date > DateTime.Today &&
                       (c.Fecha_Rescision == null || c.Fecha_Rescision.Value.Date > DateTime.Today)
                   )
                   .OrderBy(c => c.Fecha_Inicio)
                   .ToList();

            ViewBag.ContratosVigentes = contratosVigentes;
        }
        else
        {
            ViewBag.ContratosVigentes = null; // No hay contratos vigentes
        }



        if (contrato.Id_Inquilino != null)
        {
            var inquilino = _repositorioInquilino.ObtenerPorId(contrato.Id_Inquilino);

            if (inquilino != null)
            {
                ViewBag.InquilinoSeleccionado = inquilino;
                Console.WriteLine($"Inquilino: {ViewBag.InquilinoSeleccionado.Nombre}");
            }
            else
            {
                ViewBag.InquilinoSeleccionado = null; // No se encontró el inquilino
            }
        }
        return View(contrato); // Si hay errores, vuelve al formulario con los datos
    }


    [HttpGet]
    [Authorize(Policy = "SoloAdmin")]
    public IActionResult Delete(int id)
    {
        try
        {
            var contrato = _repositorioContrato.ObtenerPorId(id);
            if (contrato == null)
            {
                return NotFound(); // Si no se encuentra el contrato, devuelve un error 404
            }
            else
            {
                _repositorioContrato.Baja(id); // Elimina el contrato
                TempData["Success"] = "Contrato Eliminado correctamente.";
                return RedirectToAction("Index"); // Redirige a la lista de contratos
            }
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"Error al obtener el contrato: {ex.Message}");
            return BadRequest("Error al obtener el contrato."); // Manejo de errores
        }

    }

    [HttpGet]
    public IActionResult Edit(int id)
    {

        var contrato = _repositorioContrato.ObtenerPorId(id);
        if (contrato == null)
        {
            return NotFound(); // Si no se encuentra el contrato, devuelve un error 404
        }
        ViewBag.Inquilinos = _repositorioInquilino.ObtenerTodos() ?? new List<Inquilino>();
        if (contrato.Id_Inquilino != null)
        {
            var inquilino = _repositorioInquilino.ObtenerPorId(contrato.Id_Inquilino);

            if (inquilino != null)
            {
                ViewBag.InquilinoSeleccionado = inquilino;
                Console.WriteLine($"Inquilino: {ViewBag.InquilinoSeleccionado.Nombre}");
            }
            else
            {
                ViewBag.InquilinoSeleccionado = null; // No se encontró el inquilino
            }
        }

        var res = _repositorioContrato.ObtenerContratosPorInmueble(contrato.Id_Inmueble);
        if (res != null && res.Count > 0)
        {
            // filtrar solo los contratos cuya fecha fin sea mayor a la fecha actual
            var contratosVigentes = res
                   .Where(c =>
                       c.Fecha_Fin.Date > DateTime.Today &&
                       (c.Fecha_Rescision == null || c.Fecha_Rescision.Value.Date > DateTime.Today)
                   )
                   .OrderBy(c => c.Fecha_Inicio)
                   .ToList();

            ViewBag.ContratosVigentes = contratosVigentes;
        }
        else
        {
            ViewBag.ContratosVigentes = null; // No hay contratos vigentes
        }

        if (contrato.Id_Inmueble != null)
        {
            var inmueble = _repositorioInmueble.ObtenerPorId(contrato.Id_Inmueble);

            if (inmueble != null)
            {
                ViewBag.inmuebleSeleccionado = inmueble;
                Console.WriteLine("inmueble DEL CREATE: " + ViewBag.inmuebleSeleccionado.Direccion);
            }
            else
            {
                ViewBag.InquilinoSeleccionado = null; // No se encontró el inquilino
            }
        }

        // Calcular meses de diferencia
        if (contrato.Fecha_Inicio != null && contrato.Fecha_Fin != null)
        {
            int meses = ((contrato.Fecha_Fin.Year - contrato.Fecha_Inicio.Year) * 12)
                       + contrato.Fecha_Fin.Month - contrato.Fecha_Inicio.Month;
            ViewBag.MesesDuracion = meses;
        }
        return View(contrato); // Devuelve la vista de edición con el contrato seleccionado
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Contrato contrato)
    {

        ModelState.Remove("id_Inquilino"); // <-- Ignoramos esa validación
        ModelState.Remove("Inquilino"); // <-- Ignoramos esa validación
        ModelState.Remove("Inmueble"); // <-- Ignoramos esa validación
        ModelState.Remove("Creado_En"); // <-- Ignoramos esa validación
        ModelState.Remove("Actualizado_En"); // <-- Ignoramos esa validación
        ModelState.Remove("Fecha_Rescision"); // <-- Ignoramos esa validación
        ModelState.Remove("Multa"); // <-- Ignoramos esa validación

        var contratoOriginal = _repositorioContrato.ObtenerPorId(contrato.Id);
        if (contratoOriginal == null)
        {
            return NotFound(); // Si no se encuentra el contrato, devuelve un error 404
        }

        var contratos = _repositorioContrato.ObtenerContratosPorInmueble(contrato.Id_Inmueble);

        bool haySolapamiento = contratos.Any(c =>
{
    DateTime fechaFinContratoExistente = c.Fecha_Rescision ?? c.Fecha_Fin;
    return ((contrato.Fecha_Inicio <= fechaFinContratoExistente && contrato.Fecha_Fin >= c.Fecha_Inicio) && c.Id != contrato.Id);
});



        if (contrato.Fecha_Inicio.Date < DateTime.Today)
        {
            ModelState.AddModelError("Fecha_Inicio", "La fecha de inicio no puede ser menor a hoy.");
        }
        if (contrato.Fecha_Fin.Date < contrato.Fecha_Inicio.Date)
        {
            ModelState.AddModelError("Fecha_Fin", "La fecha de fin no puede ser menor que la fecha de inicio.");
        }
        if (haySolapamiento)
        {
            ModelState.AddModelError("Fecha_Inicio", "Las fechas del contrato se solapan con otro ya existente.");
            ModelState.AddModelError("Fecha_Fin", "Las fechas del contrato se solapan con otro ya existente.");
        }

        if (ModelState.IsValid) // Verifica si los datos cumplen con las validaciones
        {
            _repositorioContrato.Modificacion(contrato); // Guarda en la BD
            TempData["Success"] = "Contrato editado correctamente.";
            return RedirectToAction("Index"); // Redirige a la lista
        }



        var res = _repositorioContrato.ObtenerContratosPorInmueble(contrato.Id_Inmueble);
        if (res != null && res.Count > 0)
        {
            var contratosVigentes = res
                   .Where(c =>
                       c.Fecha_Fin.Date > DateTime.Today &&
                       (c.Fecha_Rescision == null || c.Fecha_Rescision.Value.Date > DateTime.Today)
                   )
                   .OrderBy(c => c.Fecha_Inicio)
                   .ToList();

            ViewBag.ContratosVigentes = contratosVigentes;
        }
        else
        {
            ViewBag.ContratosVigentes = null; // No hay contratos vigentes
        }



        if (contrato.Id_Inquilino != null)
        {
            var inquilino = _repositorioInquilino.ObtenerPorId(contrato.Id_Inquilino);

            if (inquilino != null)
            {
                ViewBag.InquilinoSeleccionado = inquilino;
                Console.WriteLine("INQUILINO DEL CREATE: " + ViewBag.InquilinoSeleccionado.Nombre);
            }
            else
            {
                ViewBag.InquilinoSeleccionado = null; // No se encontró el inquilino
            }
        }


        if (contrato.Id_Inmueble != null)
        {
            var inmueble = _repositorioInmueble.ObtenerPorId(contrato.Id_Inmueble);

            if (inmueble != null)
            {
                ViewBag.inmuebleSeleccionado = inmueble;
                Console.WriteLine("inmueble DEL CREATE: " + ViewBag.inmuebleSeleccionado.Direccion);
            }
            else
            {
                ViewBag.InquilinoSeleccionado = null; // No se encontró el inquilino
            }
        }

        // Calcular meses de diferencia
        if (contratoOriginal.Fecha_Inicio != null && contratoOriginal.Fecha_Fin != null)
        {
            int meses = ((contratoOriginal.Fecha_Fin.Year - contratoOriginal.Fecha_Inicio.Year) * 12)
                       + contratoOriginal.Fecha_Fin.Month - contratoOriginal.Fecha_Inicio.Month;
            ViewBag.MesesDuracion = meses;
        }
        return View(contratoOriginal); // Si hay errores, vuelve al formulario con los datos
    }

    [HttpGet]
    public IActionResult Details(int id)
    {
        var contrato = _repositorioContrato.ObtenerPorId(id);
        if (contrato == null)
        {
            return NotFound();
        }
        var pagos = _repositorioPago.ObtenerPagosPorContrato(id);

        ViewBag.PagosAdeudados = 0m; // El 'm' indica que es decima
        var hoy = DateTime.Today;
        foreach (var pago in pagos)
        {
            Console.WriteLine("anulado" + pago.Anulado);
            if (!pago.Anulado && pago.Fecha_Vencimiento < hoy && pago.FechaPago == null) // Solo sumamos si NO está anulado
            {
                ViewBag.PagosAdeudados += pago.Importe;
            }

        }
        ViewBag.PagosRestantes = 0m;
        foreach (var pago in pagos)
        {
            if (!pago.Anulado && pago.FechaPago == null) // Solo sumamos si NO está anulado
            {
                ViewBag.PagosRestantes += pago.Importe;
                Console.WriteLine("PagosRestantes5555555555:       " + ViewBag.PagosRestantes);
            }

        }
        Console.WriteLine("DEUDA TOTAL: " + ViewBag.PagosAdeudados);


        return View(contrato);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RescindirConfirmado(int id, DateTime fechaRescision)
    {
        // 1. Recupero el contrato
        var contrato = _repositorioContrato.ObtenerPorId(id);
        if (contrato == null)
            return NotFound();

        // 2. Verifico que no esté ya rescindido
        if (contrato.Fecha_Rescision != null)
        {
            TempData["Error"] = "Este contrato ya fue rescindido.";
            return RedirectToAction("Details", new { id });
        }

        // 3. Compruebo que la fecha sea válida
        if (fechaRescision < contrato.Fecha_Inicio || fechaRescision > contrato.Fecha_Fin)
        {
            TempData["Error"] = "La fecha de rescisión debe estar entre la fecha de inicio y fin del contrato.";
            return RedirectToAction("Rescindir", new { id });
        }

        // 4. Calculo meses transcurridos completos
        int mesesTranscurridos =
            (fechaRescision.Year - contrato.Fecha_Inicio.Year) * 12
          + (fechaRescision.Month - contrato.Fecha_Inicio.Month);
        // Si el día de rescisión es anterior al día de inicio, no contamos ese mes
        if (fechaRescision.Day < contrato.Fecha_Inicio.Day)
            mesesTranscurridos--;
        mesesTranscurridos = Math.Max(0, mesesTranscurridos);

        // 5. Calculo meses totales del contrato
        int mesesTotales =
            (contrato.Fecha_Fin.Year - contrato.Fecha_Inicio.Year) * 12
          + (contrato.Fecha_Fin.Month - contrato.Fecha_Inicio.Month);
        if (contrato.Fecha_Fin.Day < contrato.Fecha_Inicio.Day)
            mesesTotales--;
        mesesTotales = Math.Max(1, mesesTotales); // al menos 1 mes

        // 6. Aplico la regla de negocio: 
        //    menos de la mitad → 2 meses de multa, sino 1 mes.
        int mesesMulta = mesesTranscurridos < (mesesTotales / 2.0) ? 2 : 1;
        decimal multa = mesesMulta * contrato.Monto;

        // 7. Marco la rescisión en el contrato
        contrato.Fecha_Rescision = fechaRescision;
        contrato.Multa = multa;
        // contrato.Usuario_Finalizador_Id  = /* ID del usuario logueado, p.ej. User.FindFirstValue(ClaimTypes.NameIdentifier) */;
        contrato.Actualizado_En = DateTime.Now;

        // 8. Persisto los cambios
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        _repositorioContrato.Modificacion(contrato, userId);

        _repositorioPago.AnularPagosFueraDeRango(
        contrato.Id,
        contrato.Fecha_Rescision.Value
        );

        int ultimoNumero = _repositorioPago.ObtenerUltimoNumeroPago(contrato.Id);
        var pago = new Pago
        {
            NumeroPago = ultimoNumero + 1,
            Fecha_Vencimiento = contrato.Fecha_Rescision.Value.AddDays(1),
            Importe = (decimal)contrato.Multa,
            ContratoId = contrato.Id,
            Anulado = false,
            Concepto = "Multa por rescisión anticipada"
        };
        _repositorioPago.Alta(pago); // Guarda el pago en la BD


        TempData["Success"] = "Contrato Rescindido correctamente.";
        return RedirectToAction("Details", new { id });
    }

    [HttpGet]
    public IActionResult Renovar(int id)
    {
        var contratoOriginal = _repositorioContrato.ObtenerPorId(id);
        if (contratoOriginal == null) return NotFound();

        Contrato modelo = new Contrato
        {
            Id_Inquilino = contratoOriginal.Id_Inquilino,
            Id_Inmueble = contratoOriginal.Id_Inmueble,
            Fecha_Inicio = contratoOriginal.Fecha_Rescision.HasValue
    ? contratoOriginal.Fecha_Rescision.Value.AddDays(1)
    : contratoOriginal.Fecha_Fin.AddDays(1),
            Fecha_Fin = contratoOriginal.Fecha_Fin.AddYears(1),   // sugerencia
            Monto = contratoOriginal.Monto // opcional
        };



        var inquilino = _repositorioInquilino.ObtenerPorId(contratoOriginal.Id_Inquilino);

        if (inquilino != null)
        {
            ViewBag.InquilinoSeleccionado = inquilino;
        }
        else
        {
            ViewBag.InquilinoSeleccionado = null; // No se encontró el inquilino
        }

        var inmueble = _repositorioInmueble.ObtenerPorId(contratoOriginal.Id_Inmueble);
        if (inmueble != null)
        {
            ViewBag.inmuebleSeleccionado = inmueble;
        }
        else
        {
            ViewBag.InquilinoSeleccionado = null; // No se encontró el inquilino
        }


        var res = _repositorioContrato.ObtenerContratosPorInmueble(contratoOriginal.Id_Inmueble);
        if (res != null && res.Count > 0)
        {
            var contratosVigentes = res
                   .Where(c =>
                       c.Fecha_Fin.Date > DateTime.Today &&
                       (c.Fecha_Rescision == null || c.Fecha_Rescision.Value.Date > DateTime.Today)
                   )
                   .OrderBy(c => c.Fecha_Inicio)
                   .ToList();

            ViewBag.ContratosVigentes = contratosVigentes;
        }
        else
        {
            ViewBag.ContratosVigentes = null; // No hay contratos vigentes
        }

        // Calcular meses de diferencia
        if (modelo.Fecha_Inicio != null && modelo.Fecha_Fin != null)
        {
            int meses = ((modelo.Fecha_Fin.Year - modelo.Fecha_Inicio.Year) * 12)
                       + modelo.Fecha_Fin.Month - modelo.Fecha_Inicio.Month;
            ViewBag.MesesDuracion = meses;
        }

        ViewBag.contratoOriginal = contratoOriginal; // Pasar el contrato original a la vista
        return View(modelo);
    }


    [HttpPost]

    [ValidateAntiForgeryToken]
    public IActionResult Renovar(Contrato contrato)
    {
        // Ignorar ciertas validaciones del modelo
        ModelState.Remove("Inquilino");
        ModelState.Remove("Inmueble");
        ModelState.Remove("Creado_En");
        ModelState.Remove("Actualizado_En");
        ModelState.Remove("Fecha_Rescision");
        ModelState.Remove("Multa");

        // Debug: Datos recibidos
        Console.WriteLine("Datos recibidos en Create CONTRATO:");
        Console.WriteLine($"Id_Inmueble: {contrato.Id_Inmueble}");
        Console.WriteLine($"Id_Inquilino: {contrato.Id_Inquilino}");
        Console.WriteLine($"Monto: {contrato.Monto}");
        Console.WriteLine($"Fecha_Inicio: {contrato.Fecha_Inicio}");
        Console.WriteLine($"Fecha_Fin: {contrato.Fecha_Fin}");

        // Obtener datos relacionados
        var contratoOriginal = _repositorioContrato.ObtenerPorId(contrato.Id);
        var contratos = _repositorioContrato.ObtenerContratosPorInmueble(contrato.Id_Inmueble);
        var inmueble = _repositorioInmueble.ObtenerPorId(contrato.Id_Inmueble);

        // Validar existencia y disponibilidad del inmueble
        if (inmueble == null || !inmueble.Disponible)
        {
            return NotFound();
        }

        // Validar solapamiento con otros contratos
        bool haySolapamiento = contratos.Any(c =>
        {
            DateTime fechaFinContratoExistente = c.Fecha_Rescision ?? c.Fecha_Fin;
            return (contrato.Fecha_Inicio <= fechaFinContratoExistente && contrato.Fecha_Fin >= c.Fecha_Inicio);
        });

        // Validaciones de fechas
        if (contrato.Fecha_Inicio.Date < DateTime.Today)
        {
            ModelState.AddModelError("Fecha_Inicio", "La fecha de inicio no puede ser menor a hoy.");
        }

        if (contrato.Fecha_Fin.Date < contrato.Fecha_Inicio.Date)
        {
            ModelState.AddModelError("Fecha_Fin", "La fecha de fin no puede ser menor que la fecha de inicio.");
        }

        if (haySolapamiento)
        {
            ModelState.AddModelError("Fecha_Inicio", "Las fechas del contrato se solapan con otro ya existente.");
            ModelState.AddModelError("Fecha_Fin", "Las fechas del contrato se solapan con otro ya existente.");
        }

        // Si el modelo es válido, guardar el contrato y generar pagos
        if (ModelState.IsValid)
        {
            int contratoId = _repositorioContrato.Alta(contrato);
            contrato.Id = contratoId;

            int meses = ((contrato.Fecha_Fin.Year - contrato.Fecha_Inicio.Year) * 12)
                        + contrato.Fecha_Fin.Month - contrato.Fecha_Inicio.Month;

            for (int i = 0; i < meses; i++)
            {
                var pago = new Pago
                {
                    NumeroPago = i + 1,
                    Fecha_Vencimiento = contrato.Fecha_Inicio.AddMonths(i).AddDays(1),
                    Importe = contrato.Monto,
                    ContratoId = contrato.Id,
                    Anulado = false,
                    Concepto = "Alquiler Mensual"
                };

                Console.WriteLine("INFORMACION DE PAGOS");
                Console.WriteLine($"NumeroPago: {pago.NumeroPago}");
                Console.WriteLine($"Fecha_Vencimiento: {pago.Fecha_Vencimiento}");

                _repositorioPago.Alta(pago);
                Console.WriteLine($"Mes {i + 1}: {contrato.Fecha_Inicio.AddMonths(i).ToString("MMMM yyyy")}");
            }
            TempData["Success"] = "Contrato Renovado correctamente.";
            return RedirectToAction("Index");
        }

        // Si hay errores, volver al formulario mostrando contratos vigentes si hay
        Console.WriteLine("id inmueble de contrato 555555555555555555: " + contrato.Id_Inmueble);
        var res = _repositorioContrato.ObtenerContratosPorInmueble(contrato.Id_Inmueble);
        // inicializar viewbag
        ViewBag.ContratosVigentes = null;
        if (res != null && res.Count > 0)
        {
            Console.WriteLine("Contratos obtenidos para el inmueble:");
            foreach (var c in res)
            {
                Console.WriteLine($"ID: {c.Id}, Inicio: {c.Fecha_Inicio}, Fin: {c.Fecha_Fin}, Rescisión: {c.Fecha_Rescision}");
            }
            var contratosVigentes = res
                .Where(c =>
                    c.Fecha_Fin.Date > DateTime.Today &&
                    (c.Fecha_Rescision == null || c.Fecha_Rescision.Value.Date > DateTime.Today))
                .OrderBy(c => c.Fecha_Inicio)
                .ToList();



            ViewBag.ContratosVigentes = contratosVigentes;

            Console.WriteLine("Contratos FILTRADOS EN VIEWBAG:");
            foreach (var c in ViewBag.ContratosVigentes)
            {
                Console.WriteLine($"ID: {c.Id}, Inicio: {c.Fecha_Inicio}, Fin: {c.Fecha_Fin}, Rescisión: {c.Fecha_Rescision}");
            }
        }
        else
        {
            ViewBag.ContratosVigentes = null;
        }

        // Mostrar información del inquilino seleccionado
        if (contrato.Id_Inquilino != null)
        {
            var inquilino = _repositorioInquilino.ObtenerPorId(contrato.Id_Inquilino);
            if (inquilino != null)
            {
                ViewBag.InquilinoSeleccionado = inquilino;
                Console.WriteLine($"Inquilino: {ViewBag.InquilinoSeleccionado.Nombre}");
            }
            else
            {
                ViewBag.InquilinoSeleccionado = null;
            }
        }

        // Mostrar información del inmueble seleccionado
        if (inmueble != null)
        {
            ViewBag.inmuebleSeleccionado = inmueble;
            Console.WriteLine("inmueble DEL CREATE: " + ViewBag.inmuebleSeleccionado.Direccion);
        }
        else
        {
            ViewBag.InquilinoSeleccionado = null;
        }

        ViewBag.contratoOriginal = contratoOriginal;

        return View(contrato);
    }






    [HttpGet("Contrato/ValidarFechasDisponibles")]
    public IActionResult ValidarFechasDisponibles(int idInmueble, DateTime fechaInicio, DateTime fechaFin)
    {
        var contratos = _repositorioContrato.ObtenerContratosPorInmueble(idInmueble);

        // Verificar solapamiento con otros contratos
        bool haySolapamiento = contratos.Any(c =>
        {
            DateTime fechaFinContratoExistente = c.Fecha_Rescision ?? c.Fecha_Fin;
            return (fechaInicio <= fechaFinContratoExistente && fechaFin >= c.Fecha_Inicio);
        });


        if (haySolapamiento)
        {
            return Json(new { Disponible = false, Mensaje = "Ya existe un contrato vigente en ese rango de fechas." });
        }
        return Json(new { Disponible = true });
    }

}
