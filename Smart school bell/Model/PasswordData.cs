using System.Security.Cryptography;
using System.Text;

namespace Smart_school_bell.Model
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

        public static bool ChekEmptyPasswors()
        {
            using (var context = new DatabaseContext())
            {
                foreach (var password in context.Passwords)
                {
                    if(password.Password != null)
                        return false;
                }
            }
            return true;
        }

        public static string GetHashString(string s)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(s);

            MD5CryptoServiceProvider CSP =
                new MD5CryptoServiceProvider();

            byte[] byteHash = CSP.ComputeHash(bytes);

            string hash = string.Empty;

            foreach (byte b in byteHash)
                hash += string.Format("{0:x2}", b);

            return hash;
        }

        public static PasswordData GetPasswordData()
        {
            using (var context = new DatabaseContext())
            {
                foreach (var passwordData in context.Passwords)
                {
                    return passwordData;
                }
            }

            return null;
        }
    }
}