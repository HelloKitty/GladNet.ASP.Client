using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.ASP.Client.Lib
{
	public class WebResponseNetworkMessageAdapter : IResponseMessage
	{
		public NetSendable<PacketPayload> Payload { get; }

		public WebResponseNetworkMessageAdapter(PacketPayload payload)
		{
			Payload = new NetSendable<PacketPayload>(payload);
		}
	}
}
