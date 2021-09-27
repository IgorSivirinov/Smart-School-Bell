using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SmartSchoolBellCore.Model
{
    public class PasswordData
    {
        public int Id { get; set; }
        public string Password { get; set; }

        public PasswordData(){}

        public PasswordData(string password)
        {
            Password = password;
        }

        public static bool CheckEmptyPasswords()
        {
            using var context = new DatabaseContext();
            return Enumerable.All(context.Passwords, password => password.Password == null);
        }

        public static string GetHashString(string s)
        {
            var bytes = Encoding.Unicode.GetBytes(s);

            var csp = new MD5CryptoServiceProvider();

            var byteHash = csp.ComputeHash(bytes);

            return byteHash.Aggregate(string.Empty, (current, b) => current + $"{b:x2}");
        }

        public static async Task<PasswordData> GetPasswordData()
        {
            await using var context = new DatabaseContext();

            return (await context.Passwords.ToListAsync()).FirstOrDefault();
        }
    }
}