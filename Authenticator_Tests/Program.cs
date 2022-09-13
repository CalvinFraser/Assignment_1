using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using AuthenticationInterface;
using Newtonsoft.Json;
using System.IO;

namespace Authenticator_Tests
{

    internal class Test
    {
       public string name { get; set; }
       public string password { get; set; }
       public string subject { get; set; }
    }

    internal class Program
    {

        static int AppendJsonToFile(Test test)
        {
            if(File.Exists("json.txt"))
            {
        
                StreamReader SR = new StreamReader("json.txt");
                string jsonFile = SR.ReadToEnd();
                List<Test> items = JsonConvert.DeserializeObject<List<Test>>(jsonFile);

                items.Add(test);
  
                SR.Close();
                File.WriteAllText("json.txt", string.Empty);
                string jsonSave = JsonConvert.SerializeObject(items, Formatting.Indented);
   
                File.WriteAllText("json.txt", jsonSave);
                return 1; 

            }
            else
            {

                List<Test> items = new List<Test>();
                items.Add(test);
                string jsonSave = JsonConvert.SerializeObject(items, Formatting.Indented);
                File.WriteAllText("json.txt", jsonSave);
                return 1;
            }

        }

        static void Main(string[] args)
        {
            /*
            ChannelFactory<IAuthentication> channelFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            IAuthentication channel; 
            string URL = "net.tcp://localhost:8100/Authenticator";

            channelFactory = new ChannelFactory<IAuthentication>(tcp, URL);
            channel = channelFactory.CreateChannel();

            string temp = channel.Register("Calvin", "1234ABC");
            int token = channel.Login("Calvin", "1234ABC");
            string valid = channel.Valdiate(token);

            Console.WriteLine(temp);
            Console.WriteLine("Token is " + token);
            Console.WriteLine(valid);
            */

            Test test = new Test();
            test.name = "Calvin";
            test.password = "1234ABC";
            test.subject = "Computing";

            AppendJsonToFile(test);



            test.name = "Bob";
            test.password = "122324ABC";
            test.subject = "Science";


            AppendJsonToFile(test);


            test.name = "Kat";
            test.password = "Babe";
            test.subject = "Cutie";


            AppendJsonToFile(test);

            StreamReader SR = new StreamReader("json.txt");
            string jsonFile = SR.ReadToEnd();
            List<Test> items = JsonConvert.DeserializeObject<List<Test>>(jsonFile);
            SR.Close();
            foreach (Test item in items)
            {
                Console.WriteLine(item.name);
            }
            


            Console.ReadLine();
            File.Delete("json.txt");

        }
    }
}
