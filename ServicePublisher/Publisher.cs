using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APICommon;
using AuthenticationInterface;
using System.ServiceModel;
using RestSharp;
using Newtonsoft.Json;
namespace ServicePublisher
{
    public class Publisher
    {

        static ChannelFactory<IAuthentication> channelFactory;
        static NetTcpBinding tcp = new NetTcpBinding();
        static IAuthentication channel;
        static int token;
        public static void displayMenu()
        {
            Console.WriteLine("####### PUBLISHING CONSOLE #######");
            Console.WriteLine("####### 1. Register        #######");
            Console.WriteLine("####### 2. Login           #######");
            Console.WriteLine("####### 3. Publish         #######");
            Console.WriteLine("####### 4. Unpublish       #######");
            Console.WriteLine("####### 5. Show endpoints  #######");
            Console.WriteLine("####### 0. Exit            #######");
            Console.Write("\n####### User input: ");
        }

        public static int getInput()
        {
            string input = Console.ReadLine();
            int output;
            if (int.TryParse(input, out output))
            {
                if (output >= 0 && output <= 5)
                { return output;  }
                else
                {
                    Console.WriteLine("Please enter a value between 0 and 5");
                    Console.ReadLine();
                    return -1;
                }
           
                
            }
            else
            {
                Console.WriteLine("Please enter a valid integer next time");
                Console.ReadLine();
                return -1; 
            }
        }

        public static void handleRegister()
        {
            Console.WriteLine("****** Register ******");
            Console.Write("\nPlease enter a username: ");
            string username = Console.ReadLine();
            Console.Write("\nPlease enter a password: ");
            string password = Console.ReadLine();

            try
            {
                string result = channel.Register(username, password);
                Console.Write(result);
                Console.ReadLine();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        public static void handleLogin()
        {


            if (token > -1)
            {
                Console.WriteLine("Already logged in.");
                Console.ReadLine();
                return;
            }
            Console.WriteLine("****** Login ******");
            Console.Write("\nPlease your username: ");
            string username = Console.ReadLine();
            Console.Write("Please your password: ");
            string password = Console.ReadLine();
            
           

            try
            {
                token = channel.Login(username, password);
                Console.Write("Logged in successfully. Token is " + token);
                Console.ReadLine();
            }
            catch (Exception e)
            {
                token = -1; 
                Console.WriteLine(e.Message);
            }
        }
        public static void handlePublish()
        {
            if (token != -1)
            {
                Console.WriteLine("****** Publish ******");
                RegisterData RD = new RegisterData();

                Console.Write("Service name: ");
                string name = Console.ReadLine();
                Console.Write("Service Description: ");
                string desc = Console.ReadLine();
                Console.Write("Service api endpoint: ");
                string endpoint = Console.ReadLine();
                Console.Write("How many operands: ");
                string noOperands = Console.ReadLine();
                Console.Write("Operand type: ");
                string type = Console.ReadLine();

                RD.Name = name;
                RD.Description = desc;
                RD.API_Endpoint = endpoint;
                RD.No_operands = int.Parse(noOperands);
                RD.Operand_type = type;

                string send = JsonConvert.SerializeObject(RD);

                try
                {
                    RestClient RC = new RestClient("http://localhost:9000/");
                    RestRequest RR = new RestRequest("api/Publish?token=" + token, Method.Patch);
                    RR.AddBody(send);

                    RestResponse response = RC.Patch(RR);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {


                        CommonOutput CO = JsonConvert.DeserializeObject<CommonOutput>(response.Content);
                        if (string.Compare(CO.Status, "Service added", true) == 0)
                        {
                            Console.WriteLine("Service added");
                        }
                        else if (string.Compare(CO.Status, "Duplicate", true) == 0)
                        {
                            Console.WriteLine("Service at the endpoint already exists");
                        }
                        else
                        {
                            Console.WriteLine("Service unable to be added. Reason: " + CO.Reason);
                        }
                    }
                    else
                    {
                        Console.Write("Something went wrong." + response.StatusCode);
                    }
                    Console.ReadLine();
                }
                catch(Exception e)
                {
                    if(e is System.Net.Http.HttpRequestException)
                    {
                        Console.WriteLine("Unable to connect to registry server");
                        Console.ReadLine();
                    }
                }
            }
            else
            {
                Console.WriteLine("Please login before publishing");
                Console.ReadLine();
            }
        }
        public static void handleUnpublish()
        {
            if (token != -1)
            {
                Console.WriteLine("****** Unpublish ******");
                RegisterData RD = new RegisterData();

                Console.Write("Service api endpoint to remove: ");
                string endpoint = Console.ReadLine();
                try
                {
                    RestClient RC = new RestClient("http://localhost:9000/");
                    RestRequest RR = new RestRequest("api/Unpublish?token=" + token + "&endpoint=" + endpoint, Method.Patch);
                    

                    RestResponse response = RC.Patch(RR);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {


                        CommonOutput CO = JsonConvert.DeserializeObject<CommonOutput>(response.Content);
                        if (string.Compare(CO.Status, "Allowed", true) == 0)
                        {
                            Console.WriteLine("Service removed");
                        }
                        else
                        {
                            Console.WriteLine("Service unable to be removed. Reason: " + CO.Reason);
                        }
                    }
                    else
                    {
                        Console.Write("Something went wrong." + response.StatusCode);
                    }
                    Console.ReadLine();
                }
                catch (Exception e)
                {
                    if (e is System.Net.Http.HttpRequestException)
                    {
                        Console.WriteLine("Unable to connect to registry server");
                        Console.ReadLine();
                    }
                }
            }
            else
            {
                Console.WriteLine("Please login before publishing");
                Console.ReadLine();
            }

        }


        static void handleShowEndpoints()
        {
            if(token > -1)
            {
                try
                {
                    Console.WriteLine("****** All endpoints in registry ******");
                    RestClient RC = new RestClient("http://localhost:9000/");
                    RestRequest RR = new RestRequest("api/All?token=" + token, Method.Get);
                    RestResponse response = RC.Get(RR);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {


                        CommonOutput CO = JsonConvert.DeserializeObject<CommonOutput>(response.Content);
                        if (string.Compare(CO.Status, "Allowed", true) == 0)
                        {
                            List<RegisterData> services = JsonConvert.DeserializeObject<List<RegisterData>>(CO.Result);

                            foreach(RegisterData service in services)
                            {
                                Console.WriteLine("-> " + service.API_Endpoint);

                            }
                        }
                        else
                        {
                            Console.WriteLine("Services unable to be displayed" + CO.Reason);
                        }
                    }
                    else
                    {
                        Console.Write("Something went wrong." + response.StatusCode);
                    }
                    Console.ReadLine();
                }
                catch (Exception e)
                {
                    if (e is System.Net.Http.HttpRequestException)
                    {
                        Console.WriteLine("Unable to connect to registry server");
                        Console.ReadLine();
                    }
                }

            }
            else
            {
                Console.WriteLine("Please login before publishing");
                Console.ReadLine();
            }
        }
        static void Main(string[] args)
        {
            bool running = true;
            int userInput;
            token = -1;
            try
            {
                string URL = "net.tcp://localhost:8100/Authenticator";

                channelFactory = new ChannelFactory<IAuthentication>(tcp, URL);
                channel = channelFactory.CreateChannel();

                channel.Login("Admin", "test");
                while (running)
                {
                    displayMenu();
                    userInput = getInput();
                    if (userInput >= 0)
                    {
                        switch (userInput)
                        {
                            case 0: { running = false; break; }
                            case 1: { handleRegister(); break; }
                            case 2: { handleLogin(); break; }
                            case 3: { handlePublish(); break; }
                            case 4: { handleUnpublish(); break; }
                            case 5: { handleShowEndpoints(); break; }

                        }
                    }

                    Console.Clear();
                }
            }

            catch (Exception e)
            {
                if (e is EndpointNotFoundException)
                {
                    Console.WriteLine("Could not connect to authenticator. Please check server\n");
                    Console.ReadLine();
                    return;
                }
                else
                {
                    Console.WriteLine(e.InnerException);
                    Console.ReadLine();
                    return;
                }
            }


        }
    }
}
