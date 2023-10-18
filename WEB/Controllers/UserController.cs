using Microsoft.AspNetCore.Mvc;
using Communication.DTOs;
using Communication.Models;
using Communication.Services;
using Communication.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace WEB.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly SUser _userService;
        private readonly UserManager<MUser> _userManager;
        private readonly SignInManager<MUser> _signInManager;
        public UserController(Context context, UserManager<MUser> userManager, SignInManager<MUser> signInManager)
        {
            _userService = new SUser(context, userManager);
            _userManager = userManager;
            _signInManager = signInManager;

        }
        public IActionResult Index()
        {
           
            var list =  _userService.GetUsers();
            ViewBag.Users = list;
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var obj = _userService.GetUser(id);
            if (obj == null)
                return RedirectToAction("Index");

            UserDTO res = new UserDTO
            {
                Id = obj.Id,
                Email = obj.Email,
                Files = obj.Files,
                Name = obj.Name,
                UserName = obj.UserName,
            };

            return View(res);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(MUser user)
        {
            var res = await _signInManager.PasswordSignInAsync(user.UserName, user.PasswordHash, true, false);
            if (res.Succeeded)
            {
                return RedirectToAction("Index", "File");
            }
            return View(user);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Create(MUser user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(user);

                var res = await _userService.Create(user);

                return RedirectToAction("Index", "Home");
            }
            catch(Exception ex)
            {
                return View(user);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UserDTO userDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(userDTO);

                var user = _userService.GetUser(userDTO.Id);
                user.UserName = userDTO.UserName;
                user.Name = userDTO.Name;
                var res = await _userManager.UpdateAsync(user);
                if (res.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                    throw new Exception(res.Errors.First().Description);
            }
            catch (Exception ex)
            {
                return View(userDTO);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePassword(string Id, string password)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(Id);
                if (user == null)
                {
                    throw new Exception("Usuário não encontrado");
                }
                string token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var res = await _userManager.ResetPasswordAsync(user, token, password);

                if (!res.Succeeded)
                    throw new Exception("Erro ao atualizar senha");

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
