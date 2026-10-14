using Wiring.Controls;
using Wiring.Devices;

namespace Wiring
{
    class Program
    {
        static void Main()
        {
            Button button = new Button();
            Motor motor = new Motor();
            Lamp lamp = new Lamp();

            button.Press();
        }
    }
}
