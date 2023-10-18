using Communication.Constants;
using Communication.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Utils
{
    public class Helpers
    {
        public static string GetUserId(HttpContext context)
        {
            var header = context.Request.Headers["Authorization"].FirstOrDefault();
            if (header == null)
                return null;
            var res = header.Split(' ')[1];
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(res);
            var jti = jwtSecurityToken.Claims.First(claim => claim.Type == "unique_name").Value;
            return jti;
        }

        public static string GetUserFolder(string userId)
        {
            return Path.Combine(Folders.ROOT, userId);
        }
        public static string GetFilePath(string folder, string fileName, string fileId)
        {
            if (string.IsNullOrEmpty(folder) ||
                string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(fileId))
                throw new Exception("Invalid parameters");

            string ext = Path.GetExtension(fileName);
            return Path.Combine(folder, fileId + ext);
        }
    }
}
