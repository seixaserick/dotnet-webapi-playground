using System.Security.Cryptography;
using System.Text;
namespace Services
{
    public static class HashService
    {

        public static string HashMD5(string inputString)
        {
            var inputStringByteArray = Encoding.UTF8.GetBytes(inputString); //expect input string in UTF8 then convert it in a byte array
            var hashAlgorithm = MD5.Create();
            var data = hashAlgorithm.ComputeHash(inputStringByteArray);
            var hashResult = Convert.ToHexString(data);
            return hashResult;
        }


    }
}
