using Microsoft.AspNetCore.Identity;
using Communication.Contexts;
using Communication.Models;
using Microsoft.EntityFrameworkCore;

namespace Communication.Services
{
    public class SFile
    {
        private readonly Context _context;
        private readonly UserManager<MUser> _userManager;
        private readonly SUser _userService;
        public SFile(Context context, UserManager<MUser> userManager)
        {
            this._context = context;
            this._userManager = userManager;
            this._userService = new SUser(_context, _userManager);
        }

        public string Create(MFile file)
        {
            if (file == null)
                throw new Exception("Invalid File");

            if (string.IsNullOrEmpty(file.UserId))
                throw new Exception("User is not defined");

            _context.Files.Add(file);
            _context.SaveChanges();
            return file.Id;
        }
        public bool Update(MFile file)
        {
            if (file == null)
                throw new Exception("Invalid File");

            if (string.IsNullOrEmpty(file.UserId))
                throw new Exception("User is not defined");

            _context.Files.Update(file);
            _context.SaveChanges();
            return true;
        }
        public bool Remove(MFile file)
        {
            if (file == null)
                throw new Exception("Invalid File");

            if (string.IsNullOrEmpty(file.UserId))
                throw new Exception("User is not defined");

            _context.Files.Remove(file);
            _context.SaveChanges();
            return true;
        }
        public MFile? Get(string fileId)
        {
            return _context.Files.FirstOrDefault(x => x.Id == fileId);
        }
        public async Task<List<MFile>> GetFiles(string userId)
        {
            bool isAdmin = await _userService.CheckRole(userId, "Admin");
            if (isAdmin)
            {
                return _context.Files.Include(x => x.User).ToList();
            }
            else
            {
                return _context.Files.Where(x => x.UserId == userId).Include(x => x.User).ToList();
            }
        }
    }
}
