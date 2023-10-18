using Microsoft.AspNetCore.Identity;
using Communication.Contexts;
using Communication.Models;
using Communication.DTOs;

namespace Communication.Services
{
    public class SUser
    {
        private readonly Context _context;
        private readonly UserManager<MUser> _userManager;
        public SUser(Context context, UserManager<MUser> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }
        public  MUser? GetUser(string UserId)
        {
            return _context.Users.Where(x => x.Id == UserId).FirstOrDefault();
        }
        public List<UserDTO> GetUsers()
        {
            var x = _context.Users.ToList();
            List<UserDTO> users = new List<UserDTO>();
            x.ForEach(x =>
            {
                users.Add(new UserDTO
                {
                    Email = x.Email,
                    Files = x.Files,
                    Id = x.Id,
                    Name = x.Name,
                    UserName = x.UserName,
                });
            });
            return users;
        }
        public async Task<bool> CheckRole(string UserId, string Role)
        {
            var user = GetUser(UserId);
            if (user == null)
                throw new Exception("User not Found");
            return await _userManager.IsInRoleAsync(user, Role);

        }
        public async Task<bool> Create(MUser user)
        {
            if (string.IsNullOrEmpty(user.PasswordHash))
                throw new Exception("Invalid password");

            var res = await _userManager.CreateAsync(user, user.PasswordHash);
            if (res.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Normal");
                return true;
            }
            else
                throw new Exception(res.Errors.First().Description);
        }
    }
}
