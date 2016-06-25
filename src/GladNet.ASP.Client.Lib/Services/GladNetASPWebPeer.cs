using GladNet.Common;
using GladNet.Serializer;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.ASP.Client.Lib
{
	public class GladNetASPWebPeer : RestClient
	{
		//Consider this object as the layer below things like ClientPeer
		//in the GladNet2 API. The act as interfacing with the App layer, in this case HTTP, and
		//deal with other functions and only pass formatted messages into GladNet2 Peers.

		private IDeserializerStrategy deserializer { get; }

		public GladNetASPWebPeer(ClientPeer gladNetPeer, string baseURL, string contentTypeString, INetworkMessageReceiver reciever, IDeserializerStrategy deserializationStrat)
			: base(baseURL)
		{
			if (gladNetPeer == null)
				throw new ArgumentNullException(nameof(gladNetPeer), $"The {gladNetPeer} cannot be null. It is required to complete the network stack of the GladNet API.");

			deserializer = deserializationStrat;
		}
	}
}
