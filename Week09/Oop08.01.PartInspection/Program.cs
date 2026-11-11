using PartInspection.Models;

namespace PartInspection
{
    class Program
    {
        static void Main()
        {
            // diameters measured at the inspection station; nominal 20.00 mm, tolerance +-0.05 mm
            List<Part> parts = new List<Part>
            {
                new Part("P-101", 20.01),
                new Part("P-102", 19.93),
                new Part("P-103", 20.04),
                new Part("P-104", 20.07),
                new Part("P-105", 19.98),
                new Part("P-106", 20.02)
            };
        }
    }
}
