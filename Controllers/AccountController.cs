// Controllers/AccountController.cs
using System.Security.Claims;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_Avgustin.Models;
using Microsoft.AspNetCore.Authorization;

namespace Inmobiliaria_Avgustin.Controllers
{
    public class AccountController : Controller
    {
        private readonly IRepositorioUsuario _repoUsuario;

        public AccountController(IRepositorioUsuario repoUsuario)
        {
            _repoUsuario = repoUsuario;
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous] // Permite acceso sin estar autenticado
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel vm, string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            // datos recibidos en el formulario
            Console.WriteLine($"Login DATOS ENVIADOS");
            Console.WriteLine($"Email: {vm.Email}");
            Console.WriteLine($"Password: {vm.Password}");

            if (!ModelState.IsValid)
                return View(vm);



            Console.WriteLine($"LOGIN 2");

            // 1) Buscamos el usuario por email
            var usuario = _repoUsuario.ObtenerPorEmail(vm.Email);
            if (usuario == null)
            {
                Console.WriteLine($"LOGIN 2.5");
                ModelState.AddModelError("Email", "Email o contraseña incorrectos");
                ModelState.AddModelError("Password", "Email o contraseña incorrectos");
                return View(vm);
            }
            Console.WriteLine($"LOGIN 3");

            // 2) Verificamos la contraseña con BCrypt

            if (!BCrypt.Net.BCrypt.Verify(vm.Password, usuario.PasswordHash))
            {
                Console.WriteLine($"LOGIN 3.5");
                Console.WriteLine($"CLAVE HASH: {usuario.PasswordHash}");
                Console.WriteLine($"CLAVE INGRESADA: {vm.Password}");
                ModelState.AddModelError("Email", "Email o contraseña incorrectos");
                ModelState.AddModelError("Password", "Email o contraseña incorrectos");
                return View(vm);
            }
            Console.WriteLine($"LOGIN 4");

            // 3) Creamos las claims (información que guardará la cookie)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Nombre + " " + usuario.Apellido),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            // 4) Creamos el identity y el principal
            var identity = new ClaimsIdentity(
                                claims,
                                CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // 5) Firmamos (genera la cookie)
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            // 6) Redirigimos al returnUrl o al Home
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Elimina la cookie de autenticación
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
