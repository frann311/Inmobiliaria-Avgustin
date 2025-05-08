using Inmobiliaria_Avgustin.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria_Avgustin.Controllers
{

    public class ImagenController : Controller
    {


        private readonly IRepositorioImagen repositorio;
        private readonly IWebHostEnvironment _env;

        public ImagenController(IRepositorioImagen repo, IWebHostEnvironment env)
        {
            repositorio = repo;
            _env = env;
        }

        // Muestra la galería de un inmueble
        // GET /Imagenes/Index/5
        public IActionResult Index(int inmuebleId)
        {
            ViewBag.InmuebleId = inmuebleId;
            var imgs = repositorio.BuscarPorInmueble(inmuebleId);
            return View(imgs);
        }


        [HttpPost]
        public async Task<IActionResult> Alta(int id, List<IFormFile> Imagen)
        {
            Console.WriteLine($"Subiendo imágenes... recibidas: {(Imagen?.Count ?? 0)}");
            // 1) Validación mínima
            if (Imagen == null || Imagen.Count == 0)
            {
                TempData["Error"] = "Seleccione al menos una imagen.";
                return RedirectToAction("Details", "Inmueble", new { id });
            }
            int countImg = repositorio.CantidadImagenes(id);
            if (countImg >= 5)
            {
                TempData["Error"] = "No se pueden subir más de 5 imágenes.";
                return RedirectToAction("Details", "Inmueble", new { id });
            }
            Console.WriteLine("Subiendo imagenes...2");
            // 2) Determinar carpeta física: wwwroot/Uploads/Inmuebles/{id}/
            string wwwRoot = _env.WebRootPath;
            string carpeta = Path.Combine(wwwRoot, "Uploads", "Inmuebles", id.ToString());
            Directory.CreateDirectory(carpeta); // crea toda la jerarquía si hace falta
            Console.WriteLine("Subiendo imagenes...3" + carpeta);


            foreach (var file in Imagen)
            {
                if (file.Length > 0)
                {
                    // 3.1) Nombre único con GUID + extensión
                    var ext = Path.GetExtension(file.FileName);
                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var fullPath = Path.Combine(carpeta, fileName);

                    // 3.2) Guardar en disco de forma asíncrona
                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    // 4) Persistir en BD solo la URL relativa
                    var img = new Imagen
                    {
                        InmuebleId = id,
                        Url = $"/Uploads/Inmuebles/{id}/{fileName}",
                        EsPortada = (countImg == 0) // Si es la primera imagen, marcarla como portada
                    };
                    Console.WriteLine("Subiendo imagenes...4" + img.Url);
                    repositorio.Alta(img);
                }
            }
            // 5) Redirigir a Details para que recargue y muestre las nuevas imágenes
            TempData["Success"] = "Se subieron las imágenes correctamente.";
            return RedirectToAction("Edit", "Inmueble", new { id });
        }

        // POST: Inmueble/Eliminar/5
        [HttpGet]
        // [Authorize(Policy = "Administrador")]
        public ActionResult Eliminar(int id)
        {
            try
            {
                Console.WriteLine("Eliminando imagen... id: " + id);
                //TODO: Eliminar el archivo físico
                Imagen entidad = repositorio.ObtenerPorId(id);
                if (entidad == null)
                {
                    return NotFound();
                }
                repositorio.Baja(id);
                if (entidad.EsPortada)
                {
                    // Buscar otra imagen del mismo inmueble
                    var otras = repositorio.BuscarPorInmueble(entidad.InmuebleId)
                                           .Where(i => i.Id != id)
                                           .ToList();

                    if (otras.Any())
                    {
                        // Asignar portada a la primera encontrada
                        repositorio.MarcarComoPortada(otras.First().Id);
                    }
                }
                TempData["Success"] = "Se Elimino la imágene correctamente.";
                return RedirectToAction("Edit", "Inmueble", new { id = entidad.InmuebleId });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult CambiarPortada(int id)
        {
            try
            {
                // Obtener la imagen seleccionada
                var imagenSeleccionada = repositorio.ObtenerPorId(id);
                if (imagenSeleccionada == null)
                {
                    return NotFound();
                }

                // Desmarcar todas las imágenes del mismo inmueble
                repositorio.QuitarPortadaActual(imagenSeleccionada.InmuebleId);

                // Marcar la imagen seleccionada como portada
                repositorio.MarcarComoPortada(id);

                TempData["Success"] = "Se cambió la imagen de portada correctamente.";
                return RedirectToAction("Edit", "Inmueble", new { id = imagenSeleccionada.InmuebleId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }

}