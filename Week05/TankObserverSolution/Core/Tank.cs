namespace TankObserver.Core
{
    public class Tank : IVoyeurable
    {
        public double Level { get; private set; }
        private List<IVoyeur> voyeurs = new List<IVoyeur>();

        public void Fill(double litres)
        {
            Level = Level + litres;
            Console.WriteLine("filling " + litres + " l");
            SendMessage();
        }

        public void AddVoyeur(IVoyeur voyeur)
        {
            voyeurs.Add(voyeur);
        }

        public void RemoveVoyeur(IVoyeur voyeur)
        {
            if (voyeurs.Contains(voyeur))
            {
                voyeurs.Remove(voyeur);
            }
        }

        public void SendMessage()
        {
            foreach (IVoyeur v in voyeurs)
            {
            v.UpdateSelf(Level);
            }
        }
    }
}
