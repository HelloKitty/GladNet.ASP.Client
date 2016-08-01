using GladNet.Common;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.ASP.Client.Lib.Tests
{
	[TestFixture]
	public static class WebPeerClientMessageSenderTests
	{
		[Test]
		public static void Test_Ctor_Doesnt_Throw_On_Correct_Parameters()
		{
			//assert
			WebPeerClientMessageSender sender = new WebPeerClientMessageSender(Mock.Of<IWebRequestHandlerStrategy>());
		}

		[Test]
		public static void Test_Ctor_Throws_On_Null_Parameters()
		{
			//assert
			Assert.Throws<ArgumentNullException>(() => new WebPeerClientMessageSender(null));
		}

		[Test]
		[TestCase(OperationType.Event, false)]
		[TestCase(OperationType.Request, true)]
		[TestCase(OperationType.Response, false)]
		public static void Test_CanSend_Only_Indicates_Request_Is_Valid(OperationType opType, bool expectedResult)
		{
			//arrange
			WebPeerClientMessageSender sender = new WebPeerClientMessageSender(Mock.Of<IWebRequestHandlerStrategy>());

			//assert
			Assert.AreEqual(expectedResult, sender.CanSend(opType));
		}

		[Test]
		[TestCase(OperationType.Event, SendResult.Invalid)]
		[TestCase(OperationType.Request, SendResult.Sent)]
		[TestCase(OperationType.Response, SendResult.Invalid)]
		public static void Test_SendMessage_Indicates_Invalid_If_Send_Other_Than_Request(OperationType opType, SendResult expectedResult)
		{
			//arrange

			//We have to setup the enqueue service to indicate sent for this test
			Mock<IWebRequestHandlerStrategy> handlerStrat = new Mock<IWebRequestHandlerStrategy>();
			handlerStrat.Setup(x => x.EnqueueRequest(It.IsAny<PacketPayload>()))
				.Returns(SendResult.Sent);

			WebPeerClientMessageSender sender = new WebPeerClientMessageSender(handlerStrat.Object);

			//act: try to send
			SendResult result = sender.TrySendMessage(opType, Mock.Of<PacketPayload>(), DeliveryMethod.ReliableDiscardStale);

			//assert
			Assert.AreEqual(expectedResult, result);
		}
	}
}
