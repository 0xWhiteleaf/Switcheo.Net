using System;

namespace Switcheo.Net.Exceptions
{
    public class RpcClientBusyNodesException : Exception
	{
        public RpcClientBusyNodesException() { }

        public RpcClientBusyNodesException(string message): base(message) { }
    }
}