using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApplication.BusinessLogic.UserLogin
{
    public class ClientInfo
    {

        // Bu classta clientlere ait ns tutulabilir.

        private string _name, _email;
        private string _password;
        private string _ipAddress;

        public string IpAdress
        {
            get => _ipAddress;
            set { }
        }

        public ClientInfo(string ipAddress)
        {
            _ipAddress = ipAddress;
        }

        public override string ToString()
        {
            return IpAdress;
        }
    }
}
