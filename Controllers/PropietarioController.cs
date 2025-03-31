
using Inmobiliaria_Avgustin.Models;
using Microsoft.AspNetCore.Mvc;

public class PropietarioController : Controller
{
    private readonly IRepositorioPropietario repo;

    // Inyección de dependencias
    public PropietarioController(IRepositorioPropietario repo)
    {
        this.repo = repo;
    }
    // METODOS

    public IActionResult Index()
    {
        var lista = repo.ObtenerTodos();
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