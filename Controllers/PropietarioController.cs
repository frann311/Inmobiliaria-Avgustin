
using Inmobiliaria_Avgustin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class PropietarioController : Controller
{
    private readonly IRepositorioPropietario repo;
    private readonly IRepositorioInmueble repoInmueble;

    // Inyección de dependencias
    public PropietarioController(IRepositorioPropietario repo, IRepositorioInmueble repoInmueble)
    {
        this.repoInmueble = repoInmueble;
        this.repo = repo;
    }
    // METODOS

    public IActionResult Index(int page = 1)
    {
        const int pageSize = 10;
        var lista = repo.ObtenerTodosPaginado(page, pageSize); // Página actual
        int total = repo.contarPropietarios(); // Total de registros

        int totalPages = (int)Math.Ceiling((double)total / pageSize);

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
    public IActionResult Create(Propietario propietario)
    {
        if (ModelState.IsValid) // Verifica si los datos cumplen con las validaciones
        {
            repo.Alta(propietario); // Guarda en la BD
            return RedirectToAction("Index"); // Redirige a la lista
        }
        return View(propietario); // Si hay errores, vuelve al formulario con los datos
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var propietario = repo.ObtenerPorId(id);
        if (propietario == null)
        {
            return NotFound(); // Si no se encuentra el propietario, devuelve un error 404
        }
        return View(propietario); // Devuelve la vista de edición con el propietario encontrado
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Propietario propietario)
    {
        if (ModelState.IsValid)
        {
            repo.Modificacion(propietario); // Guarda los cambios en la BD
            return RedirectToAction("Index"); // Redirige a la lista
        }
        return View(propietario); // Si hay errores, vuelve al formulario con los datos
    }

    [HttpGet]
    [Authorize(Policy = "SoloAdmin")]
    public IActionResult Delete(int id)
    {

        var propietario = repo.ObtenerPorId(id);
        if (propietario == null)
        {
            return NotFound(); // Si no se encuentra el propietario, devuelve un error 404
        }
        repo.Baja(id); // Elimina el propietario de la BD
        return RedirectToAction("Index"); // Redirige a la lista
    }

    [Route("Propietario/Buscar/{q}", Name = "BuscarPropietario")]
    public IActionResult Buscar(string q)
    {
        try
        {
            var res = repo.BuscarPorNombre(q);
            return Json(new { Datos = res });
        }
        catch (Exception ex)
        {
            return Json(new { Error = ex.Message });
        }
    }

    [HttpGet]
    public IActionResult Details(int id)
    {
        var propietario = repo.ObtenerPorId(id);
        if (propietario == null)
        {
            return NotFound(); // Si no se encuentra el propietario, devuelve un error 404
        }
        var inmuebles = repoInmueble.ObtenerPorPropietario(id);
        ViewBag.Inmuebles = inmuebles; // Pasa la lista de inmuebles a la vista
        return View(propietario); // Devuelve la vista de detalle con el propietario encontrado
    }





    // public IActionResult TestConexion()
    // {
    //     try
    //     {
    //         ViewBag.ConexionExitosa = repo.ProbarConexion();
    //     }
    //     catch (Exception ex)
    //     {
    //         ViewBag.ConexionExitosa = false;
    //         ViewBag.ErrorMessage = ex.Message;
    //     }
    //     return View();
    // }
}