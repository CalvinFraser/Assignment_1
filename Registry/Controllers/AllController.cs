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
    public class AllController : ApiController
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

        public CommonOutput Get(int token)
        {
            CommonOutput output = new CommonOutput();
            try
            {
                if (validate(token))
                {
                    StreamReader SR = new StreamReader(FILENAME);
                    string jsonFile = SR.ReadToEnd();


                    output.Status = "Allowed";
                    output.Reason = "Authorised";
                    output.Result = jsonFile;
                    SR.Close();

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
            catch (Exception e)
            {
                output.Status = "Unknown error occured";
                output.Result = e.Message;
                output.Result = "Null";
                return output;
            }
        }

    }
}
