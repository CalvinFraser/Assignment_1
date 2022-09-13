using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using APICommon;
using AuthenticationInterface;
using System.ServiceModel;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Registry.Controllers
{
    public class PublishController : ApiController
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

        static int WriteJsonToFile(RegisterData service)
        {
            if (File.Exists(FILENAME))
            {

                StreamReader SR = new StreamReader(FILENAME);
                string jsonFile = SR.ReadToEnd();
                List<RegisterData> items = JsonConvert.DeserializeObject<List<RegisterData>>(jsonFile);
                string jsonSave;
                foreach (RegisterData Service in items)
                {
                    if(string.Compare(Service.API_Endpoint, service.API_Endpoint, true) == 0)
                    {
                        SR.Close();
                        File.WriteAllText(FILENAME, string.Empty);
                        jsonSave = JsonConvert.SerializeObject(items, Formatting.Indented);

                        File.WriteAllText(FILENAME, jsonSave);
                        return -1; 
                    }
                }


                items.Add(service);

                SR.Close();
                File.WriteAllText(FILENAME, string.Empty);
                jsonSave = JsonConvert.SerializeObject(items, Formatting.Indented);

                File.WriteAllText(FILENAME, jsonSave);
                return 1;

            }
            else
            {

                List<RegisterData> items = new List<RegisterData>();
                items.Add(service);
                string jsonSave = JsonConvert.SerializeObject(items, Formatting.Indented);
                File.WriteAllText(FILENAME, jsonSave);
                return 1;
            }

        }



        public CommonOutput Patch([FromBody] RegisterData service, int token)
        {
            CommonOutput output = new CommonOutput();

            try
            {
                if (validate(token))
                {

                    if (WriteJsonToFile(service) > 0)
                    { 
                    output.Status = "Service added";
                    output.Reason = "Authorised";
                    }
                    else
                    {
                        output.Status = "Duplicate";
                        output.Reason = "Service already exists";
                    }

                }
                else
                {
                    output.Status = "Denied";
                    output.Reason = "Unauthorised";
                }
                
                return output; 

            }catch(Exception e)
            {
                output.Status = "Unknown error";
                output.Reason = e.Message;
                return output; 

            }

        }

    }
}
