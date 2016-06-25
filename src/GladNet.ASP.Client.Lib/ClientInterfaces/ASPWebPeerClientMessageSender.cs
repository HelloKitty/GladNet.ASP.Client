using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GladNet.Common;
using RestSharp;
using GladNet.Serializer;

namespace GladNet.ASP.Client.Lib
{
	public class ASPWebPeerClientMessageSender : IClientPeerNetworkMessageSender
	{
		private IRestClient httpClient { get; }

		private ISerializerStrategy serializer { get; }

		private IWebRequestHandlerStrategy requestHandler { get; }

		public ASPWebPeerClientMessageSender(IRestClient client, ISerializerStrategy serializationStrat, IWebRequestHandlerStrategy requestService)
		{
			httpClient = client;
			requestHandler = requestService;
			serializer = serializationStrat;
		}

		public SendResult SendRequest(PacketPayload payload, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			byte[] serializedData = serializer.Serialize(payload);

			//Right now we just ignore the payload args
			//TODO: Implement support for args

			//Requests are sent to ASP controlls based on the payload type names.
			return requestHandler.EnqueueRequest(serializedData, httpClient, payload.GetType().Name);
		}

		public SendResult SendRequest<TPacketType>(TPacketType payload) where TPacketType : PacketPayload, IStaticPayloadParameters
		{
			byte[] serializedData = serializer.Serialize(payload);

			//Right now we just ignore the payload args
			//TODO: Implement support for args

			//Requests are sent to ASP controlls based on the payload type names.
			return requestHandler.EnqueueRequest(serializedData, httpClient, payload.GetType().Name);
		}
	}
}
