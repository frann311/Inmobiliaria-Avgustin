using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_Avgustin.Models;
using System.Security.Claims;


// Solo administradores pueden acceder a todo este controlador
[Authorize(Roles = "Administrador")]
public class UsuarioController : Controller
{
    private readonly IRepositorioUsuario _repoUsuario;

    public UsuarioController(IRepositorioUsuario repoUsuario)
    {
        this._repoUsuario = repoUsuario;
    }

    // GET: /Usuario

    public IActionResult Index()
    {
        // 1) Recuperar todos los usuarios
        var lista = _repoUsuario.ObtenerTodos();
        return View(lista);
    }

    // GET: /Usuario/Create
    [HttpGet]
    public IActionResult Create()
    {
        Console.WriteLine("Creando nuevo usuario...");
        return View();
    }

    // POST: /Usuario/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Usuario modelo)
    {
        ModelState.Remove("PasswordHash"); // No queremos que se valide el hash de la contraseña
        ModelState.Remove("Creado_En"); // No queremos que se valide la fecha de creación
        ModelState.Remove("Actualizado_En"); // No queremos que se valide la fecha de actualización
        if (!ModelState.IsValid)
        {
            Console.WriteLine("Modelo no válido");
            return View(modelo);
        }
        // 1) Verificar si el email ya existe
        var usuarioExistente = _repoUsuario.ObtenerPorEmail(modelo.Email);
        if (usuarioExistente != null)
        {
            ModelState.AddModelError(nameof(modelo.Email), "El email ya está en uso.");
            return View(modelo);
        }


        // 2) Hashear la contraseña antes de guardar
        modelo.PasswordHash = BCrypt.Net.BCrypt.HashPassword(modelo.Password);

        // 3) Insertar nuevo usuario
        int nuevoId = _repoUsuario.Alta(modelo);
        TempData["Success"] = "Se Creo el Usuario Correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Usuario/Edit/5
    public IActionResult Edit(int id)
    {
        var u = _repoUsuario.ObtenerPorId(id);
        if (u == null) return NotFound();
        var vm = new UsuarioEditViewModel
        {
            Id = u.Id,
            Nombre = u.Nombre,
            Apellido = u.Apellido,
            Email = u.Email,
            Rol = u.Rol
        };
        return View(vm);
    }
    // public IActionResult Edit(int id)
    // {
    //     Console.WriteLine("Editando usuario con ID: " + id);
    //     var usuario = _repoUsuario.ObtenerPorId(id);
    //     if (usuario == null) return NotFound();
    //     Console.WriteLine("Editando usuario con USUARIO.ID: " + usuario.Id);
    //     return View(usuario);
    // }

    // POST: /Usuario/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]


    public IActionResult Edit(UsuarioEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var u = _repoUsuario.ObtenerPorId(vm.Id);
        if (u == null) return NotFound();
        // 1) Verificar si el email ya existe
        var usuarioExistente = _repoUsuario.ObtenerPorEmail(vm.Email);
        if (usuarioExistente != null)
        {
            // Si el email ya existe y no es el mismo que el del usuario actual, agregar error
            if (usuarioExistente.Id != vm.Id)
            {
                ModelState.AddModelError(nameof(vm.Email), "El email ya está en uso.");
                return View(vm);
            }
        }

        // Sólo si enviaron algo en NewPassword
        if (!string.IsNullOrEmpty(vm.NewPassword))
        {
            if (vm.NewPassword != vm.ConfirmPassword)
            {
                ModelState.AddModelError(nameof(vm.ConfirmPassword), "La confirmación no coincide.");
                return View(vm);
            }
            u.PasswordHash = BCrypt.Net.BCrypt.HashPassword(vm.NewPassword);
        }

        // Actualizo datos básicos
        u.Nombre = vm.Nombre;
        u.Apellido = vm.Apellido;
        u.Email = vm.Email;
        u.Rol = vm.Rol;

        Console.WriteLine("Editando usuario con USUARIO.ID: " + u.Id);

        _repoUsuario.Modificacion(u);
        TempData["Success"] = "Se Edito el Usuario Correctamente.";
        return RedirectToAction(nameof(Index));
    }


    // public IActionResult Edit(int id, Usuario modelo)
    // {
    //     if (id != modelo.Id) return BadRequest();
    //     ModelState.Remove("Password");
    //     ModelState.Remove("PasswordHash"); // No queremos que se valide el hash de la contraseña
    //     ModelState.Remove("Creado_En"); // No queremos que se valide la fecha de creación
    //     ModelState.Remove("Actualizado_En"); // No queremos que se valide la fecha de actualización

    //     // 1) Quitamos cualquier error de validación en el campo Password
    //     if (ModelState.ContainsKey("Password"))
    //     {
    //         ModelState["Password"].Errors.Clear();
    //     }


    //     if (!ModelState.IsValid)
    //         return View(modelo);

    //     // Si ingresaron nueva contraseña, la hasheamos
    //     if (!string.IsNullOrEmpty(modelo.Password))
    //     {
    //         modelo.PasswordHash = BCrypt.Net.BCrypt.HashPassword(modelo.Password);
    //     }

    //     _repoUsuario.Modificacion(modelo);
    //     return RedirectToAction(nameof(Index));
    // }

    // GET: /Usuario/Delete/5
    [HttpGet]
    public IActionResult Delete(int id)
    {
        Console.WriteLine("Eliminando usuario con ID: " + id);
        var usuario = _repoUsuario.ObtenerPorId(id);
        if (usuario == null) return NotFound();
        return View(usuario); // Mostrar vista de confirmación
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        _repoUsuario.Baja(id);
        TempData["Success"] = "Se elimino el Usuario Correctamente.";
        return RedirectToAction(nameof(Index));
    }




}

