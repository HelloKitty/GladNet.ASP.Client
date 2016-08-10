using GladNet.Engine.Common;
using GladNet.Message;
using GladNet.Serializer;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.ASP.Client.RestSharp.Tests
{
	[TestFixture]
	class RestSharpCurrentThreadEnqueueRequestHandlerStrategyTests
	{
		[Test]
		public void Test_Doesnt_Throw_On_Ctor()
		{
			//assert
			Assert.DoesNotThrow(() => new RestSharpCurrentThreadEnqueueRequestHandlerStrategy(@"http://www.google.com", Mock.Of<IDeserializerStrategy>(), Mock.Of<ISerializerStrategy>(), Mock.Of<INetworkMessageReceiver>(), 5, Mock.Of<INetworkMessageRouteBackService>()));
		}

		//This test might seem dumb but it was null and this will prevent regressions
		[Test]
		public void Test_Ctor_Sets_Properties_From_Ctor()
		{
			//arrange
			RestSharpCurrentThreadEnqueueRequestHandlerStrategy strat = new RestSharpCurrentThreadEnqueueRequestHandlerStrategy(@"http://www.google.com", Mock.Of<IDeserializerStrategy>(), Mock.Of<ISerializerStrategy>(), Mock.Of<INetworkMessageReceiver>(), 5, Mock.Of<INetworkMessageRouteBackService>());

			Assert.NotNull(strat.GetType().GetProperty("serializer", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetProperty).GetValue(strat, new object[0]));
		}
	}
}
