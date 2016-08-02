using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GladNet.Common;
using RestSharp;
using GladNet.Serializer;
using System.Diagnostics.CodeAnalysis;
using GladNet.Engine.Common;
using GladNet.Payload;
using GladNet.Message;

namespace GladNet.ASP.Client.Lib
{
	/// <summary>
	/// Web implementation for <see cref="IClientPeerNetworkMessageSender"/> and <see cref="INetworkMessageSender"/>
	/// that mediates the sending of <see cref="PacketPayload"/>s to web servers.
	/// </summary>
	public class WebPeerClientMessageSender : IClientPeerPayloadSender, INetworkMessageRouterService
	{
		/// <summary>
		/// Internal webrequest handling strategy for outgoing <see cref="PacketPayload"/>s.
		/// </summary>
		private IWebRequestEnqueueStrategy requestEnqueueStrat { get; }

		/// <summary>
		/// Creates an instance of the <see cref="WebPeerClientMessageSender"/> service.
		/// </summary>
		/// <param name="requestService">Implementation of the <see cref="IWebRequestEnqueueStrategy"/> that actually handles the logic for request payload handling.</param>
		public WebPeerClientMessageSender(IWebRequestEnqueueStrategy requestService)
		{
			if (requestService == null)
				throw new ArgumentNullException(nameof(requestService), $"Parameter {requestService} should not be null. Provide a non-null {nameof(IWebRequestEnqueueStrategy)}.");

			requestEnqueueStrat = requestService;
		}

		// <summary>
		/// Sends a networked request.
		/// </summary>
		/// <param name="payload"><see cref="PacketPayload"/> for the desired network request message.</param>
		/// <param name="deliveryMethod">Desired <see cref="DeliveryMethod"/> for the request. See documentation for more information.</param>
		/// <param name="encrypt">Optional: Indicates if the message should be encrypted. Default: false</param>
		/// <param name="channel">Optional: Inidicates the channel the network message should be sent on. Default: 0</param>
		/// <returns>Indication of the message send state.</returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public SendResult SendRequest(PacketPayload payload, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			if (payload == null)
				throw new ArgumentNullException(nameof(payload), $"Payload parameter {payload} must not be null.");

			//Right now we just ignore the payload args
			//TODO: Implement support for args

			//Requests are sent to ASP controlls based on the payload type names.
			return requestEnqueueStrat.EnqueueRequest(payload);
		}

		// <summary>
		/// Sends a networked request.
		/// Additionally this message/payloadtype is known to have static send parameters and those will be used in transit.
		/// </summary>
		/// <typeparam name="TPacketType">Type of the packet payload.</typeparam>
		/// <param name="payload">Payload instance to be sent in the message that contains static message parameters.</param>
		/// <returns>Indication of the message send state.</returns>
		public SendResult SendRequest<TPacketType>(TPacketType payload) 
			where TPacketType : PacketPayload, IStaticPayloadParameters
		{
			if (payload == null)
				throw new ArgumentNullException(nameof(payload), $"Payload parameter {payload} must not be null.");

			//Right now we just ignore the payload args
			//TODO: Implement support for args

			//Requests are sent to ASP controlls based on the payload type names.
			return requestEnqueueStrat.EnqueueRequest(payload);
		}

		/// <summary>
		/// Attempts to send a web request message; will fail is this message isn't a request.
		/// </summary>
		/// <param name="opType"><see cref="OperationType"/> of the message to send.</param>
		/// <param name="payload">Payload instance to be sent in the message.</param>
		/// <param name="deliveryMethod">The deseried <see cref="DeliveryMethod"/> of the message.</param>
		/// <param name="encrypt">Indicates if the message should be encrypted.</param>
		/// <param name="channel">Indicates the channel for this message to be sent over.</param>
		/// <returns>Indication of the message send state.</returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public SendResult TrySendMessage(OperationType opType, PacketPayload payload, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			if (CanSend(opType))
				return SendRequest(payload, deliveryMethod, encrypt, channel);
			else
				return SendResult.Invalid;
		}

		/// <summary>
		/// Attempts to send a web request message; will fail is this message isn't a request.
		/// Additionally this message/payloadtype is known to have static send parameters and those will be used in transit.
		/// </summary>
		/// <typeparam name="TPacketType">Type of the packet payload.</typeparam>
		/// <param name="opType"><see cref="OperationType"/> of the message to send.</param>
		/// <param name="payload">Payload instance to be sent in the message that contains static message parameters.</param>
		/// <returns>Indication of the message send state.</returns>
		public SendResult TrySendMessage<TPacketType>(OperationType opType, TPacketType payload) 
			where TPacketType : PacketPayload, IStaticPayloadParameters
		{
			if (CanSend(opType))
				return SendRequest(payload);
			else
				return SendResult.Invalid;
		}

		/// <summary>
		/// Indicates if the <see cref="OperationType"/> can be sent with this peer.
		/// </summary>
		/// <param name="opType"><see cref="OperationType"/> to check.</param>
		/// <returns>True if the peer can see the <paramref name="opType"/>.</returns>
		public bool CanSend(OperationType opType)
		{
			return opType == OperationType.Request;
		}

		public SendResult TryRouteMessage<TMessageType>(TMessageType message, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0) 
			where TMessageType : INetworkMessage, IRoutableMessage, IOperationTypeMappable
		{
			//TODO: Deal with additional parameters

			switch (message.OperationTypeMappedValue)
			{		
				case OperationType.Request:
					return requestEnqueueStrat.EnqueueRequest(message as RequestMessage); //TODO: use IRequestMessage
				case OperationType.Event:
				case OperationType.Response:
				default:
					throw new InvalidProgramException($"ASP client cannot handle message types other than {typeof(IRequestMessage).Name}.");
			}
		}
	}
}
