
using Inmobiliaria_Avgustin.Models;
using Microsoft.AspNetCore.Mvc;

public class InquilinoController : Controller
{
    private readonly IRepositorioInquilino repo;

    // Inyección de dependencias
    public InquilinoController(IRepositorioInquilino repo)
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
    public IActionResult Create(Inquilino Inquilino)
    {
        if (ModelState.IsValid) // Verifica si los datos cumplen con las validaciones
        {
            repo.Alta(Inquilino); // Guarda en la BD
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
            return RedirectToAction("Index"); // Redirige a la lista
        }
        return View(Inquilino); // Si hay errores, vuelve al formulario con los datos
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {

        var Inquilino = repo.ObtenerPorId(id);
        if (Inquilino == null)
        {
            return NotFound(); // Si no se encuentra el Inquilino, devuelve un error 404
        }
        repo.Baja(id); // Elimina el Inquilino de la BD
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