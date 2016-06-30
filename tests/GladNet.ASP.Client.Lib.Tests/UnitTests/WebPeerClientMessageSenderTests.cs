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
	}
}
