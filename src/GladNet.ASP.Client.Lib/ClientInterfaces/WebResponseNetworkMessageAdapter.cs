using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.ASP.Client.Lib
{
	/// <summary>
	/// Simple <see cref="IResponseMessage"/> adapter that implements bare minimum GladNet specification
	/// to allow for the transfer between GladNet services of a raw <see cref="PacketPayload"/>.
	/// </summary>
	public class WebResponseNetworkMessageAdapter : IResponseMessage
	{
		/// <summary>
		/// Netsendable <see cref="PacketPayload"/>.
		/// </summary>
		public NetSendable<PacketPayload> Payload { get; }

		/// <summary>
		/// Creates an adapter that allows a web request <see cref="PacketPayload"/> to fit the
		/// GladNet service specification.
		/// </summary>
		/// <param name="payload">Web request <see cref="PacketPayload"/>.</param>
		public WebResponseNetworkMessageAdapter(PacketPayload payload)
		{
			//This is lame but if we're too communicate with GladNet services we must be IResponseMessage
			//And this contains a NetSendable which in some implementations is serialized when sent.
			Payload = new NetSendable<PacketPayload>(payload);
		}
	}
}
