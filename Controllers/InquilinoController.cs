
using Inmobiliaria_Avgustin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


public class InquilinoController : Controller
{
    private readonly IRepositorioInquilino repo;
    private readonly IRepositorioContrato repoContrato;

    // Inyección de dependencias
    public InquilinoController(IRepositorioInquilino repo, IRepositorioContrato repoContrato)
    {
        this.repoContrato = repoContrato;
        this.repo = repo;
    }

    // METODOS


    public IActionResult Index(int page = 1)
    {
        int pageSize = 10; // Cantidad de inquilinos por página
        var lista = repo.obtenerTodosPaginado(page, pageSize); // Página actual
        int total = repo.CantarInquilinos(); // Total de registros
        int totalPages = (int)Math.Ceiling((double)total / pageSize); // Total de páginas
        ViewBag.Total = total;
        ViewBag.Page = page;
        ViewBag.TotalPages = totalPages;
        return View(lista);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Inquilino Inquilino)
    {
        if (ModelState.IsValid) // Verifica si los datos cumplen con las validaciones
        {
            repo.Alta(Inquilino); // Guarda en la BD
            TempData["Success"] = "Se cargo el Inquilino Correctamente.";
            return RedirectToAction("Index"); // Redirige a la lista
        }
        return View(Inquilino); // Si hay errores, vuelve al formulario con los datos
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var Inquilino = repo.ObtenerPorId(id);
        if (Inquilino == null)
        {
            return NotFound(); // Si no se encuentra el Inquilino, devuelve un error 404
        }
        return View(Inquilino); // Devuelve la vista de edición con el Inquilino encontrado
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Inquilino Inquilino)
    {
        if (ModelState.IsValid)
        {
            repo.Modificacion(Inquilino); // Guarda los cambios en la BD
            TempData["Success"] = "Se edito el Inquilino Correctamente.";
            return RedirectToAction("Index"); // Redirige a la lista
        }
        return View(Inquilino); // Si hay errores, vuelve al formulario con los datos
    }

    [HttpGet]
    [Authorize(Policy = "SoloAdmin")]
    public IActionResult Delete(int id)
    {

        var Inquilino = repo.ObtenerPorId(id);
        if (Inquilino == null)
        {
            return NotFound(); // Si no se encuentra el Inquilino, devuelve un error 404
        }
        repo.Baja(id); // Elimina el Inquilino de la BD
        TempData["Success"] = "Se elimino el Inquilino Correctamente.";
        return RedirectToAction("Index"); // Redirige a la lista
    }




    [HttpGet]
    public IActionResult Details(int id)
    {
        var Inquilino = repo.ObtenerPorId(id);
        if (Inquilino == null)
        {
            return NotFound(); // Si no se encuentra el Inquilino, devuelve un error 404
        }
        var contratos = repoContrato.ObtenerContratosPorInquilino(id); // Obtiene los contratos asociados al Inquilino
        ViewBag.Contratos = contratos; // Pasa los contratos a la vista
        return View(Inquilino); // Devuelve la vista de detalles con el Inquilino encontrado
    }

    [Route("Inquilino/Buscar/{q}", Name = "BuscarInquilino")]
    public IActionResult Buscar(string q)
    {
        Console.WriteLine("BUSCANDO INQUILINO: " + q);
        try
        {
            var res = repo.BuscarPorNombre(q);
            Console.WriteLine("ENCONTRADOS" + res.Count);
            return Json(new { Datos = res });
        }
        catch (Exception ex)
        {
            return Json(new { Error = ex.Message });
        }
    }





}