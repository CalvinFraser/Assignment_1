using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using AuthenticationInterface;
namespace Authenticator_Tests
{
    internal class Program
    {
        static void Main(string[] args)
        {

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
            Console.ReadLine();

        }
    }
}
