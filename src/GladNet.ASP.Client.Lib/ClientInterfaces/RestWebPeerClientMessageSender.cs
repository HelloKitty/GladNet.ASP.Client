using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GladNet.Common;
using RestSharp;
using GladNet.Serializer;

namespace GladNet.ASP.Client.Lib
{
	public class RestWebPeerClientMessageSender : IClientPeerNetworkMessageSender, INetworkMessageSender
	{
		private IRestClient httpClient { get; }

		private ISerializerStrategy serializer { get; }

		private IWebRequestHandlerStrategy requestHandler { get; }

		public RestWebPeerClientMessageSender(string baseURL, ISerializerStrategy serializationStrat, IWebRequestHandlerStrategy requestService)
		{
			httpClient = new RestClient(baseURL);
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

		public SendResult TrySendMessage(OperationType opType, PacketPayload payload, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			if (CanSend(opType))
				return SendRequest(payload, deliveryMethod, encrypt, channel);
			else
				return SendResult.Invalid;
		}

		public SendResult TrySendMessage<TPacketType>(OperationType opType, TPacketType payload) where TPacketType : PacketPayload, IStaticPayloadParameters
		{
			if (CanSend(opType))
				return SendRequest(payload);
			else
				return SendResult.Invalid;
		}

		public bool CanSend(OperationType opType)
		{
			return opType == OperationType.Request;
		}
	}
}
