using Communication.Constants;
using Communication.Models.NFeModel;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Xml.Linq;
using System.Xml.Serialization;

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
        public static T SerializeFromXMLString<T>(string fileXml) where T : class
        {
            var xmlDoc = XDocument.Parse(fileXml);
            var xmlString = (from d in xmlDoc.Descendants()
                             where d.Name.LocalName == typeof(T).Name
                             select d).FirstOrDefault();

            if (xmlString == null)
                throw new Exception(String.Format("Nenhum objeto NFe encontrado no arquivo {1}!", fileXml));

            var ser = XmlSerializer.FromTypes(new[] { typeof(T) })[0];
            var str = xmlString.ToString();

            using (var sr = new StringReader(str))
                return (T)ser.Deserialize(sr);
        }
    }
}
