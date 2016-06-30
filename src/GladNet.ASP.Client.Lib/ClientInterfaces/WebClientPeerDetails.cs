using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace GladNet.ASP.Client.Lib
{
	/// <summary>
	/// Web-based implementation for the GladNet <see cref="IConnectionDetails"/> contract.
	/// </summary>
	public class WebClientPeerDetails : IConnectionDetails
	{
		/// <summary>
		/// Indicates the connection ID should the server assign unique
		/// connection IDs for user-connections.
		/// </summary>
		public int ConnectionID { get; }

		/// <summary>
		/// Indicates the local port this Web/HTTP connection is going out over.
		/// Number will be negative if unavailable.
		/// </summary>
		public int LocalPort { get; }

		/// <summary>
		/// Indicates the <see cref="IPAddress"/> for the remote web service/host.
		/// </summary>
		public IPAddress RemoteIP { get; }

		/// <summary>
		/// Indicates the remote port of the web service.
		/// Usually this is standard HTTP port 80 (maybe HTTPS 443)
		/// </summary>
		public int RemotePort { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="remoteIP"></param>
		/// <param name="localPort"></param>
		/// <param name="connectionID"></param>
		public WebClientPeerDetails(string remoteIP, int localPort, int connectionID)
		{
			//TODO: Address port changes when using HTTPS

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
