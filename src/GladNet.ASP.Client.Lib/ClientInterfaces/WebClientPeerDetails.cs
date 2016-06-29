using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace GladNet.ASP.Client.Lib
{
	public class WebClientPeerDetails : IConnectionDetails
	{
		public int ConnectionID { get; private set; }

		public int LocalPort { get; private set; }

		public IPAddress RemoteIP { get; private set; }

		public int RemotePort { get; private set; }

		public WebClientPeerDetails(string remoteIP, int localPort, int connectionID)
		{
			//Ok, we're going to guess the remoteport is 80 unless we
			//see a port specificed in the remoteIP string
			int port = 80;

			//Check the IP string for {0}:{Port} format
			if (remoteIP.Contains(':'))
				port = int.Parse(remoteIP.Split(':').Last());

			//Don't know how we get this but some HTTP implementation might give us this info
			LocalPort = localPort;

			//Probably going to be 0 with HTTP connections
			ConnectionID = connectionID;

			//http://stackoverflow.com/questions/6498829/cant-parse-domain-into-ip-in-c
			IPAddress tempIP = null;
			if (!IPAddress.TryParse(remoteIP, out tempIP))
				RemoteIP = Dns.GetHostEntry(remoteIP).AddressList[0];
			else
				RemoteIP = tempIP;
		}
	}
}
