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
    public class AddThreeNumbersController : ApiController
    {
        private bool validate(int token)
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
        public ServiceProviderOutput Get(int a, int b, int c, int token)
        {
            ServiceProviderOutput SPO = new ServiceProviderOutput();


            if (validate(token))
            {
                SPO.Status = "Allowed";
                SPO.Reason = "Authorised";
                SPO.result = a + b + c;
            }
            else
            {
                SPO.Status = "Denied";
                SPO.Reason = "Unauthorised";
                SPO.result = 0;

            }

            return SPO;
        }
    }
}
