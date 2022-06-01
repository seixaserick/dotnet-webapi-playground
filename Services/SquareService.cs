
namespace Services
{
    public static class SquareService
    {

        public static int SquareAllDigits(int n)
        {

            var inputDigits = n.ToString().ToCharArray();
            string resultString = "";
            foreach (var digit in inputDigits)
            {
                int square = int.Parse(digit.ToString());
                resultString += square * square;

            }
            return int.Parse(resultString);

        }
    }
}
