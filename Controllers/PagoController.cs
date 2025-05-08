using System.Security.Claims;
using Inmobiliaria_Avgustin.Models;

using Microsoft.AspNetCore.Mvc;

public class PagoController : Controller
{
    private readonly IRepositorioPago repo;
    private readonly IRepositorioContrato repositorioContrato;

    public PagoController(IRepositorioPago repo, IRepositorioContrato repositorioContrato)
    {
        this.repo = repo;
        this.repositorioContrato = repositorioContrato;
    }


    [HttpGet]
    public IActionResult Index(int id)
    {
        Console.WriteLine("ID: " + id);
        var contrato = repositorioContrato.ObtenerPorId(id);
        Console.WriteLine("contratoid: " + contrato.Id);
        if (contrato == null)
        {
            return NotFound();
        }

        ViewBag.Contrato = contrato;
        var pagos = repo.ObtenerPagosPorContrato(id);
        return View(pagos);
    }

    [HttpGet]
    public IActionResult Anular(int id)
    {
        try
        {
            var pago = repo.ObtenerPorId(id);
            if (pago == null)
            {
                return NotFound();
            }
            if (pago.FechaPago != null)
            {
                return BadRequest("El pago ya fue realizado.");
            }
            if (pago.Anulado == true)
            {
                return BadRequest("El pago ya fue anulado.");
            }
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            repo.Anular(pago.Id, userId);

            TempData["Success"] = "Se anulo el Pago Correctamente.";
            return RedirectToAction("Index", "Pago", new { id = pago.ContratoId });
        }
        catch (Exception ex)
        {
            // Manejo de excepciones
            Console.WriteLine("Error: " + ex.Message);
            return BadRequest("Error al anular el pago.");
            throw;
        }




    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var pago = repo.ObtenerPorId(id);
        if (pago == null)
        {
            return NotFound();
        }
        if (pago == null)
        {
            return NotFound();
        }
        if (pago.FechaPago != null)
        {
            return BadRequest("El pago ya fue realizado.");
        }
        if (pago.Anulado == true)
        {
            return BadRequest("El pago fue anulado.");
        }
        return View(pago);
    }
    [HttpPost]
    public IActionResult Edit(Pago pago)
    {
        // 1. Obtener el pago existente de la base de datos
        var pagoExistente = repo.ObtenerPorId(pago.Id);
        if (pagoExistente == null)
        {
            return NotFound();
        }
        if (pagoExistente == null)
        {
            return NotFound();
        }
        if (pagoExistente.FechaPago != null)
        {
            return BadRequest("El pago ya fue realizado.");
        }
        if (pagoExistente.Anulado == true)
        {
            return BadRequest("El pago fue anulado.");
        }

        // 2. Actualizar solo los campos editables del formulario
        pagoExistente.Concepto = pago.Concepto;

        // 3. Eliminar validación solo para campos no editables
        ModelState.Clear(); // Limpia todas las validaciones

        // 4. Validar manualmente los campos necesarios
        if (string.IsNullOrEmpty(pagoExistente.Concepto))
        {
            ModelState.AddModelError("Concepto", "El concepto es requerido");
        }

        // 5. Si es válido, guardar
        if (ModelState.IsValid)
        {
            try
            {
                repo.Modificacion(pagoExistente);
                TempData["Success"] = "Se edito el Pago Correctamente.";
                return RedirectToAction("Index", "Pago", new { id = pagoExistente.ContratoId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al guardar: {ex.Message}");
            }
        }

        // 6. Si hay errores, recargar datos para la vista
        return View(pagoExistente);
    }

    [HttpGet]
    public IActionResult Pagar(int id)
    {
        var pago = repo.ObtenerPorId(id);
        if (pago == null)
        {
            return NotFound();
        }
        if (pago.FechaPago != null)
        {
            return BadRequest("El pago ya fue realizado.");
        }
        if (pago.Anulado == true)
        {
            return BadRequest("El pago fue anulado.");
        }
        var hoy = DateTime.Today;
        // datos enviados
        // Console.WriteLine("HOY FECHA PAGO" + hoy);
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        repo.Pagar(pago, hoy, userId);
        TempData["Success"] = "Se Realizo el Pago Correctamente.";
        return RedirectToAction("Index", "Pago", new { id = pago.ContratoId });
    }


    // GET: /Pago/Details/5
    public IActionResult Details(int id)
    {
        // 1) Recuperar el pago por id, incluyendo UsuarioAlta y UsuarioAnulador
        var pago = repo.ObtenerPorId(id);
        if (pago == null)
            return NotFound();

        // 2) Opcional: cargar navegación si tu repositorio no lo hace
        //    pago.UsuarioAlta     = _repoUsuario.ObtenerPorId(pago.UsuarioAltaId.Value);
        //    pago.UsuarioAnulador = pago.UsuarioAnuladorId.HasValue
        //                             ? _repoUsuario.ObtenerPorId(pago.UsuarioAnuladorId.Value)
        //                             : null;

        // 3) Pasar el modelo a la vista
        return View(pago);
    }
}