using GladNet.Common;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.ASP.Client.Lib
{
	public interface IWebRequestHandlerStrategy
	{
		SendResult EnqueueRequest(byte[] serializedRequest, IRestClient webClient, string requestName);
	}
}
