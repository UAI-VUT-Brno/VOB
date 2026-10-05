// VOB - Exercise 4, part 3 - a coffee machine, all in one file
// It works. Finding anything in it is another matter.

namespace Barista
{
    class Grinder
    {
        private int _gramsSinceCleaning;

        public bool NeedsCleaning
        {
            get { return _gramsSinceCleaning >= 100; }
        }

        public void Grind(int grams)
        {
            _gramsSinceCleaning = _gramsSinceCleaning + grams;
            Console.WriteLine("grinding " + grams + " g");
        }

        public void Clean()
        {
            _gramsSinceCleaning = 0;
        }
    }

    class Drink
    {
        public string Name { get; }
        public int WaterMl { get; }
        public int BeansG { get; }
        public int Price { get; }             // CZK

        public Drink(string name, int waterMl, int beansG, int price)
        {
            Name = name;
            WaterMl = waterMl;
            BeansG = beansG;
            Price = price;
        }
    }

    class Menu
    {
        private DrinkList _drinks;

        public Menu(DrinkList drinks)
        {
            _drinks = drinks;
        }

        public int Ask()
        {
            Console.WriteLine();
            for (int i = 1; i <= _drinks.Count; i++)
            {
                Drink d = _drinks.Get(i);
                Console.WriteLine(i + "  " + d.Name + "  " + d.Price + " CZK");
            }
            Console.WriteLine("7  refill   8  clean grinder   9  status   0  off");
            Console.Write("> ");

            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice))
            {
                Console.Write("A number, please: ");
            }
            return choice;
        }
    }

    class Container
    {
        public string Name { get; }
        public string Unit { get; }
        public int Capacity { get; }
        public int Level { get; private set; }

        public Container(string name, string unit, int capacity)
        {
            Name = name;
            Unit = unit;
            Capacity = capacity;
            Level = capacity;                 // delivered full
        }

        public bool HasEnough(int amount)
        {
            return amount <= Level;
        }

        public void Take(int amount)
        {
            Level = Level - amount;
        }

        public void Fill()
        {
            Level = Capacity;
        }

        public string Describe()
        {
            return Name + ": " + Level + " / " + Capacity + " " + Unit;
        }
    }

    class CoffeeMachine
    {
        private readonly Container _water = new Container("water", "ml", 1500);
        private readonly Container _beans = new Container("beans", "g", 250);
        private readonly Grinder _grinder = new Grinder();

        public int Earned { get; private set; }

        public void Make(Drink drink)
        {
            if (_grinder.NeedsCleaning)
            {
                Console.WriteLine("Clean the grinder first.");
                return;
            }
            if (!_water.HasEnough(drink.WaterMl))
            {
                Console.WriteLine("Not enough water.");
                return;
            }
            if (!_beans.HasEnough(drink.BeansG))
            {
                Console.WriteLine("Not enough beans.");
                return;
            }

            _grinder.Grind(drink.BeansG);
            _water.Take(drink.WaterMl);
            _beans.Take(drink.BeansG);
            Earned = Earned + drink.Price;
            Console.WriteLine("Here is your " + drink.Name + ".");
        }

        public void Refill()
        {
            _water.Fill();
            _beans.Fill();
        }

        public void CleanGrinder()
        {
            _grinder.Clean();
            Console.WriteLine("The grinder is clean.");
        }

        public void PrintStatus()
        {
            Console.WriteLine(_water.Describe());
            Console.WriteLine(_beans.Describe());
            Console.WriteLine("earned: " + Earned + " CZK");
        }
    }

    class DrinkList
    {
        private List<Drink> _drinks = new List<Drink>();

        public int Count
        {
            get { return _drinks.Count; }
        }

        public void Add(Drink drink)
        {
            _drinks.Add(drink);
        }

        public Drink Get(int number)
        {
            return _drinks[number - 1];
        }
    }

    class Program
    {
        static void Main()
        {
            DrinkList drinks = new DrinkList();
            drinks.Add(new Drink("espresso", 40, 9, 45));
            drinks.Add(new Drink("lungo", 110, 9, 50));
            drinks.Add(new Drink("doppio", 70, 18, 65));

            CoffeeMachine machine = new CoffeeMachine();
            Menu menu = new Menu(drinks);

            while (true)
            {
                int choice = menu.Ask();

                if (choice == 0)
                    break;
                else if (choice >= 1 && choice <= drinks.Count)
                    machine.Make(drinks.Get(choice));
                else if (choice == 7)
                    machine.Refill();
                else if (choice == 8)
                    machine.CleanGrinder();
                else if (choice == 9)
                    machine.PrintStatus();
                else
                    Console.WriteLine("No such choice.");
            }
        }
    }
}
