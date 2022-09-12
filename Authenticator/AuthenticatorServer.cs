using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using AuthenticationInterface;using System.Timers;

namespace Authenticator
{
    internal class AuthenticatorServer
    {
        private static System.Timers.Timer aTimer;
        static void SetTimer(int minutes)
        {
            int minsToTicks = (minutes * 60) * 1000;
            aTimer = new Timer(minsToTicks);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Authenticator.clearCache();
            Console.WriteLine("Cleared tokens at {0:HH:mm:ss.fff}",
                              e.SignalTime);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Authenticator starting....");
            int minutes = 0;
            
            Console.Write("Please enter how many minutes should pass before token cache is cleared: ");
            string tempMin = Console.ReadLine();
            if (!int.TryParse(tempMin, out minutes))
            {
                Console.WriteLine("Please enter a valid whole number!");
                Console.ReadLine();
                return; 
            }
            if(minutes > 10 || minutes < 1)
            {
                Console.WriteLine("Please enter a value between 1 and 10 minutes");
                Console.ReadLine();
            }
            Console.WriteLine("Authenticator initialising....");
            ServiceHost host;
            NetTcpBinding tcp = new NetTcpBinding();
            try
            {
                host = new ServiceHost(typeof(Authenticator));
                host.AddServiceEndpoint(typeof(IAuthentication), tcp, "net.tcp://0.0.0.0:8100/Authenticator");

                host.Open();
                SetTimer(minutes);
                Console.WriteLine("Authenticator operational.");
                while(true)
                {

                }

                host.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }

        }
    }
}
