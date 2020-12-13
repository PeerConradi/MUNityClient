using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Managing.Interop
{
    public static class LocalStorageJs
    {

        [JSInvokable]
        public static void LocalStorageChanged()
        {
            Console.WriteLine("Local storage changed!");
        }
    }
}
