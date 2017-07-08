using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace WizepipesSocketServer
{
    class AddressItem
    {
        public Socket socket;
        public byte[] buffer;
        public string strAddress;
    }
}
