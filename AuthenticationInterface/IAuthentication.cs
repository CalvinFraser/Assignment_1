using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;


namespace AuthenticationInterface
{
    [ServiceContract]
    public interface IAuthentication
    {
        [OperationContract]
        String Register(String name, String password);
        [OperationContract]
        int Login(String name, String password);
        [OperationContract]
        String Valdiate(int token);



    }
}
