using GladNet.Common;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.ASP.Client.Lib
{
	/// <summary>
	/// 
	/// </summary>
	public interface IWebRequestHandlerStrategy
	{
		SendResult EnqueueRequest(byte[] serializedRequest, string requestName);
	}
}
