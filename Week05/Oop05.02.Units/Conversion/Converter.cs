namespace Units.Conversion
{
    public class Converter
    {
        public double ToFahrenheit(double celsius)
        {
            return celsius * 9 / 5 + 32;
        }

        public double ToKelvin(double celsius)
        {
            return celsius + 273.15;
        }
    }
}
