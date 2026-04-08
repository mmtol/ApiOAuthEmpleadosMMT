using System.Security.Cryptography;
using System.Text;

namespace MvcOAuthApiEmpleados.Helpers
{
    public class HelperCrytography
    {
        public string Encrypt(string json, string key)
        {
            using (Aes aes = Aes.Create())
            {
                using (var sha256 = SHA256.Create())
                {
                    aes.Key = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
                }

                aes.GenerateIV();

                using (var ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(json);
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public string Decrypt(string json, string key)
        {
            byte[] fullCipher = Convert.FromBase64String(json);

            using (Aes aes = Aes.Create())
            {
                using (var sha256 = SHA256.Create())
                {
                    aes.Key = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
                }

                byte[] iv = new byte[16];
                Array.Copy(fullCipher, iv, iv.Length);
                aes.IV = iv;

                using (var ms = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length))
                using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
