using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using APICommon;
using AuthenticationInterface;
using System.IO;
using System.ServiceModel;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
namespace Registry.Controllers
{
    public class SearchController : ApiController
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
        static string SearchServices(string SearchDescription)
        {
            StreamReader SR = new StreamReader(FILENAME);
            string jsonFile = SR.ReadToEnd();
            List<RegisterData> searchPool = JsonConvert.DeserializeObject<List<RegisterData>>(jsonFile);
            List<RegisterData> matches = new List<RegisterData>();
            Regex rx = new Regex(SearchDescription);
            foreach (RegisterData item in searchPool)
            {
                if (rx.IsMatch(item.Description))
                {
                    matches.Add(item);
                }
            }
            string jsonSave = JsonConvert.SerializeObject(matches, Formatting.Indented);
            return jsonSave;

            SR.Close();
        }


        public CommonOutput Get(string SearchDescription, int token)
        {
            CommonOutput output = new CommonOutput();

            try
            {
                if (validate(token))
                {

                    string matches = SearchServices(SearchDescription);

                    output.Status = "Allowed";
                    output.Reason = "Authorised";
                    if (matches.Length > 0)
                    {
                        output.Result = matches;
                    }
                    else
                    {
                        output.Result = "Nil";
                    }
                }
                else
                {
                    output.Status = "Denied";
                    output.Reason = "Unauthorised";
                    output.Result = "Null";
                }

                return output;

            }
            catch (Exception e)
            {
                output.Status = "Unknown error";
                output.Reason = e.Message;
                output.Result = "Null";
                return output;

            }

        }
    }
}
