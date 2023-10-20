using System.Security.Cryptography;
using System.Text;

namespace BankAPI.Tools
{
  public class Encrypt
  {
    public static string GetSha256Has(string str)
    {
      SHA256 sha256 = SHA256.Create();
      ASCIIEncoding encoding = new ASCIIEncoding();
      byte[] stream = null!;
      StringBuilder sb = new StringBuilder();
      stream = sha256.ComputeHash(encoding.GetBytes(str));
      for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
      return sb.ToString();
    }
    public static string GetSha512Hash(string rawData)
    {
      // Create a SHA256   
      using (SHA512 sha512Hash = SHA512.Create())
      {
        // ComputeHash - returns byte array  
        byte[] bytesData = sha512Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

        // Convert byte array to a string   
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < bytesData.Length; i++)
        {
          builder.Append(bytesData[i].ToString("x2"));
        }
        return builder.ToString();
      }
    }

    private void CreatePasswordHash(string pwd, out byte[] passwordHash, out byte[] passwordSalt)
    {
      using (var hmac = new System.Security.Cryptography.HMACSHA512())
      {
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pwd)); 
      }
    }
  }
}
