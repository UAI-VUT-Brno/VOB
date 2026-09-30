// VOB - Exercise 3, block 4.2 - an alarm that can only ever send e-mail

namespace Oop0308
{
    public class EmailSender
    {
        public void Send(string message)
        {
            Console.WriteLine("[e-mail] " + message);
        }
    }

    public class SmsSender
    {
        public void Send(string message)
        {
            Console.WriteLine("[sms] " + message);
        }
    }

    public class Alarm
    {
        private EmailSender _sender;

        public Alarm(EmailSender sender)
        {
            _sender = sender;
        }

        public void Raise(string text)
        {
            _sender.Send("ALARM: " + text);
        }
    }

    class Program
    {
        static void Main()
        {
            Alarm a = new Alarm(new EmailSender());
            a.Raise("pump overheated");

            // Alarm b = new Alarm(new SmsSender());
            // b.Raise("pressure too high");
        }
    }
}
