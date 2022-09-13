using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APICommon;
using AuthenticationInterface;
using System.ServiceModel;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Web.Http;


namespace Registry.Controllers
{
    public class UnpublishController : ApiController
    {

        const string FILENAME = "PublishedServices.txt";

        private bool validate(int token)
        {
            try
            {
                ChannelFactory<IAuthentication> channelFactory;
                NetTcpBinding tcp = new NetTcpBinding();
                IAuthentication channel;
                string URL = "net.tcp://localhost:8100/Authenticator";

                channelFactory = new ChannelFactory<IAuthentication>(tcp, URL);
                channel = channelFactory.CreateChannel();

                string valid = channel.Valdiate(token);
                if (string.Compare(valid, "valid", true) == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private int removeService(string endpoint)
        {
            StreamReader SR = new StreamReader(FILENAME);
            string jsonFile = SR.ReadToEnd();
            List<RegisterData> items = JsonConvert.DeserializeObject<List<RegisterData>>(jsonFile);
            List<RegisterData> newItems = new List<RegisterData>();
            SR.Close();
            foreach (RegisterData item in items)
            {
                if(string.Compare(item.API_Endpoint, endpoint, true) != 0)
                {
                    Console.WriteLine(item.Name);
                    newItems.Add(item);
                }
            }
            File.WriteAllText(FILENAME, string.Empty);
            string jsonSave = JsonConvert.SerializeObject(newItems, Formatting.Indented);
            File.WriteAllText(FILENAME, jsonSave);
            

            return 1; 


        }
        public CommonOutput Patch(int token, string endpoint)
        {
            CommonOutput output = new CommonOutput();

            try
            {
                if (validate(token))
                {
                    removeService(endpoint);
                    output.Status = "Allowed";
                    output.Reason = "Authorised";
                    output.Result = "Service removed";
                    return output; 
                }
                else
                {
                    output.Status = "Denied";
                    output.Reason = "Unauthorised";
                    output.Result = "Null";
                    return output;
                }

            }
            catch(Exception e)
            {
                output.Status = "Unknown error occured";
                output.Reason = e.Message;
                output.Result = "Null";
                return output;
            }
        }
    }
}
