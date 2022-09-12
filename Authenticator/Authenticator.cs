using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using AuthenticationInterface;
using System.IO;

namespace Authenticator
{

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class Authenticator : IAuthentication
    {

        public static void clearCache()
        {
            File.WriteAllText("RegisteredTokens.txt", string.Empty);
        }

        public string Register(string name, string password)
        {
            string Name = name;
            string Pass = password;

            registerToFile(Name, Pass);
            return ("Account " + Name + " [" + Pass + "]" + " registered successfully");
        }

        private async void registerToFile(string name, string password)
        {
            string save = name + ":" + password;
            StreamWriter file = new StreamWriter("RegisteredLog.txt", append: true);
            await file.WriteLineAsync(save);
            file.Close();
        }

        public int Login(string name, string password)
        {

            string search = name + ":" + password;
            StreamReader file = new StreamReader("RegisteredLog.txt");
            while (!file.EndOfStream)
            {
                string line = file.ReadLine();
                if (string.Compare(search, line) == 0)
                {
                    return generateNewToken();
                }
            }
            file.Close();
            return -1;
        }

        private int generateNewToken()
        {
            Random random = new Random(((int)DateTime.Now.Ticks));
            int token = random.Next();

            saveTokenToFile(token);

            return token;
        }

        private async void saveTokenToFile(int token)
        {
            StreamWriter file = new StreamWriter("RegisteredTokens.txt", append: true);
            await file.WriteLineAsync(token.ToString());
            file.Close();
        }

        public string Valdiate(int token)
        {
            if (checkToken(token))
            {
                return "valid";
            }
            else
            {
                return "invalid";
            }
        }

        private bool checkToken(int token)
        {
            string search = token.ToString();
            StreamReader file = new StreamReader("RegisteredTokens.txt");
            while (!file.EndOfStream)
            {
                string line = file.ReadLine();
                if(string.Compare(search, line) == 0)
                {
                    file.Close();
                    return true; 
                }
            }
            file.Close();
            return false;
        }
    }
}
