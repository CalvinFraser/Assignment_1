using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.ServiceModel;
using AuthenticationInterface;
using APICommon;

namespace ServiceProvider.Controllers
{
    public class MulThreeNumbersController : ApiController
    {
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
            }catch(Exception e)
            {
                throw e; 
            }
        }
        public CommonOutput Get(int a, int b, int c,  int token)
        {
            CommonOutput SPO = new CommonOutput();

            try
            {
                if (validate(token))
                {
                    SPO.Status = "Allowed";
                    SPO.Reason = "Authorised";
                    SPO.Reason = (a * b * c).ToString();
                }
                else
                {
                    SPO.Status = "Denied";
                    SPO.Reason = "Unauthorised";
                    SPO.Result = "Null";

                }
            }
            catch(Exception e)
            {
                SPO.Status = "Unknown error occured";
                SPO.Reason = "Please check authentication server";
                SPO.Result = "Null";
            }

            return SPO;
        }
    }
}
