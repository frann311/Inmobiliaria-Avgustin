using Inmobiliaria_Avgustin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

public class InmuebleController : Controller  // Ahora coincide con el constructor
{
    private readonly IRepositorioInmueble repo;
    private readonly IRepositorioPropietario repoPropietrio;
    private readonly IRepositorioTiposImnuebles repositorioTiposInmuebles;
    private readonly IRepositorioUsosInmuebles repositorioUsosInmuebles;

    private readonly IRepositorioContrato repositorioContrato;
    private readonly IRepositorioImagen _repoImagen;

    // Constructor simplificado
    public InmuebleController(
        IRepositorioInmueble repo,
        IRepositorioPropietario repoPropietrio,
        IRepositorioTiposImnuebles repositorioTiposInmuebles,
        IRepositorioUsosInmuebles repositorioUsosInmuebles,
        IRepositorioContrato repositorioContrato,
        IRepositorioImagen repoImagen)
    {
        this.repo = repo;
        this.repoPropietrio = repoPropietrio;
        this.repositorioTiposInmuebles = repositorioTiposInmuebles;
        this.repositorioUsosInmuebles = repositorioUsosInmuebles;
        this.repositorioContrato = repositorioContrato;
        this._repoImagen = repoImagen;
    }

    // Método Index
    public IActionResult Index(bool? disponible, int? propietarioId, DateTime? fechaDesde, DateTime? fechaHasta, int? TipoId, int? UsoId, int page = 1)
    {
        const int pageSize = 10;

        // 1) Recuperar el propietario seleccionado (para mantenerlo en la vista)
        if (propietarioId.HasValue)
        {
            ViewBag.PropietarioSeleccionado = repoPropietrio.ObtenerPorId(propietarioId.Value);
        }
        // 2) Guardar valores de filtro en ViewBag para la vista
        ViewBag.Disponible = disponible;
        ViewBag.PropietarioId = propietarioId;
        ViewBag.Page = page;
        ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd"); // Para que lo tome el input date
        ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");
        ViewBag.TiposInmuebles = repositorioTiposInmuebles.ObtenerTodos() ?? new List<TiposInmuebles>();
        ViewBag.UsosInmuebles = repositorioUsosInmuebles.ObtenerTodos() ?? new List<UsosInmuebles>();
        ViewBag.TipoId = TipoId;
        ViewBag.UsoId = UsoId;
        // 3) Llamar al repositorio pasándole ambos filtros
        var inmuebles = repo.ObtenerTodosFilter(disponible, propietarioId, fechaDesde, fechaHasta, TipoId, UsoId, page, pageSize);

        // 2) Obtener el conteo total de inmuebles con esos filtros
        int totalCount = repo.ContarInmueblesFilter(
            disponible, propietarioId, fechaDesde, fechaHasta, TipoId, UsoId);

        int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        ViewBag.TotalPages = totalPages;

        return View(inmuebles);
    }

    [HttpGet]
    public IActionResult Create()
    {
        // ViewBag.TiposInmuebles = repositorioTiposInmuebles.ObtenerTodos();
        // ViewBag.UsosInmuebles = repositorioUsosInmuebles.ObtenerTodos();
        ViewBag.TiposInmuebles = repositorioTiposInmuebles.ObtenerTodos() ?? new List<TiposInmuebles>();
        ViewBag.UsosInmuebles = repositorioUsosInmuebles.ObtenerTodos() ?? new List<UsosInmuebles>();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Inmueble inmueble)
    {
        ModelState.Remove("Propietario");
        ModelState.Remove("Tipo");
        ModelState.Remove("Uso");

        if (ModelState.IsValid)
        {
            inmueble.Disponible = true;
            repo.Alta(inmueble);
            TempData["Success"] = "Se cargo el Inmueble Correctamente.";
            return RedirectToAction("Index");
        }

        ViewBag.TiposInmuebles = repositorioTiposInmuebles.ObtenerTodos() ?? new List<TiposInmuebles>();
        ViewBag.UsosInmuebles = repositorioUsosInmuebles.ObtenerTodos() ?? new List<UsosInmuebles>();
        ViewBag.PropietarioSelect = inmueble.PropietarioId != 0
            ? repoPropietrio.ObtenerPorId(inmueble.PropietarioId)
            : null;

        return View(inmueble);
    }

    [HttpGet]
    [Authorize(Policy = "SoloAdmin")]
    public IActionResult Delete(int id)
    {
        var inmueble = repo.ObtenerPorId(id);
        if (inmueble == null)
        {
            return NotFound();
        }
        repo.Baja(id);
        TempData["Success"] = "Se Elimino el Inmueble Correctamente.";
        return RedirectToAction("Index");

    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        ViewBag.TiposInmuebles = repositorioTiposInmuebles.ObtenerTodos() ?? new List<TiposInmuebles>();
        ViewBag.UsosInmuebles = repositorioUsosInmuebles.ObtenerTodos() ?? new List<UsosInmuebles>();
        var entidad = repo.ObtenerPorId(id);
        if (entidad == null)
        {
            return NotFound();
        }
        ViewBag.Propietarios = repoPropietrio.ObtenerTodos();
        ViewBag.Imagenes = _repoImagen.BuscarPorInmueble(id)
                        .OrderBy(img => img.Id) // opcional: mantener un orden
                        .ToList();

        return View(entidad);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Inmueble inmueble)
    {
        // En teoría no es necesario remover estas validaciones si usás [Required] y [Range]
        ModelState.Remove("Propietario");
        ModelState.Remove("Tipo");
        ModelState.Remove("Uso");

        var inmuebleExistente = repo.ObtenerPorId(inmueble.Id);
        if (inmuebleExistente == null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            repo.Modificacion(inmueble);
            TempData["Success"] = "Se edito el Inmueble Correctamente.";
            return RedirectToAction("Index");
        }

        // Si hay errores, volvemos a armar los datos para el formulario
        ViewBag.Propietarios = repoPropietrio.ObtenerTodos();
        ViewBag.TiposInmuebles = repositorioTiposInmuebles.ObtenerTodos() ?? new List<TiposInmuebles>();
        ViewBag.UsosInmuebles = repositorioUsosInmuebles.ObtenerTodos() ?? new List<UsosInmuebles>();
        ViewBag.Imagenes = _repoImagen.BuscarPorInmueble(inmueble.Id)
            .OrderBy(img => img.Id)
            .ToList();



        return View(inmueble);
    }

    public IActionResult Details(int id)
    {
        var inmueble = repo.ObtenerPorId(id);
        if (inmueble == null) return NotFound();

        var contratos = repositorioContrato.ObtenerContratosPorInmueble(id);
        if (contratos != null && contratos.Count > 0)
        {
            // Buscar contrato vigente (considerando rescisión como posible fecha final)
            var contratoVigente = contratos.FirstOrDefault(c =>
                DateTime.Today >= c.Fecha_Inicio &&  // Fecha actual posterior o igual a inicio
                (
                    (c.Fecha_Rescision == null && DateTime.Today <= c.Fecha_Fin) ||  // No rescindido y dentro del plazo
                    (c.Fecha_Rescision != null && DateTime.Today <= c.Fecha_Rescision)  // Rescindido pero dentro del plazo de rescisión
                )
            );

            ViewBag.contratoVigente = contratoVigente;
        }
        else
        {
            ViewBag.contratoVigente = null;
        }

        // 3) Cargar todas las imágenes desde la BD
        //    BuscarPorInmueble devuelve IList<Imagen>
        var imagenes = _repoImagen.BuscarPorInmueble(id)
                        .OrderBy(img => img.Id) // opcional: mantener un orden
                        .ToList();

        ViewBag.Imagenes = imagenes;

        return View(inmueble);
    }

    [Route("Inmueble/Buscar/{q}", Name = "BuscarInmueble")]
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

}

