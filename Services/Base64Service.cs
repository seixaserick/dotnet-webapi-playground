
namespace Services
{
    public static class Base64Service
    {

        public static string Base64Encode(string inputString)
        {
            
            var inputStringByteArray = System.Text.Encoding.UTF8.GetBytes(inputString); //expect input string in UTF8 then convert it in a byte array
            var base64EncodedOutputString = System.Convert.ToBase64String(inputStringByteArray);
            return base64EncodedOutputString;

        }



        public static string Base64Decode(string inputBase64String)
        {

            var base64DecodedBytes= System.Convert.FromBase64String(inputBase64String);
            var base64Decoded = System.Text.Encoding.UTF8.GetString(base64DecodedBytes);
            return base64Decoded;

        }
    }
}
