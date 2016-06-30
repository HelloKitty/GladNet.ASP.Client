using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace GladNet.ASP.Client.Lib.Tests
{
	[TestFixture]
	public static class WebClientPeerDetailsTests
	{
		[Test]
		public static void Test_NonNull_Ctor_Doesnt_Throw()
		{
			//arrange
			WebClientPeerDetails details = new WebClientPeerDetails(@"www.gooogle.com", -1, 0);
		}

		[Test]
		public static void Test_Domain_In_Ctor_Translates_To_IP()
		{
			//arrange
			WebClientPeerDetails details = new WebClientPeerDetails(@"www.gooogle.com", -1, 0);

			//assert: That remoteIP isn't null or default value.
			Assert.NotNull(details.RemoteIP);
			Assert.AreNotEqual(details.RemoteIP, IPAddress.None);
			Assert.AreNotEqual(details.RemoteIP, IPAddress.Any);
		}

		[Test]
		public static void Test_IPString_In_Ctor_Translates_To_IP()
		{
			//arrange
			WebClientPeerDetails details = new WebClientPeerDetails(@"127.0.0.1", -1, 0);

			//assert: That remoteIP isn't null or default value.
			Assert.NotNull(details.RemoteIP);
			Assert.AreEqual(details.RemoteIP, IPAddress.Loopback); //should be true 127.0.0.1
			Assert.AreNotEqual(details.RemoteIP, IPAddress.Any);
		}

		[Test]
		public static void Test_IPStringWithPort_In_Ctor_Translates_To_IP()
		{
			//arrange
			WebClientPeerDetails details = new WebClientPeerDetails(@"127.0.0.1:600", -1, 0);

			//assert: That remoteIP isn't null or default value.
			Assert.NotNull(details.RemoteIP);
			Assert.AreEqual(details.RemoteIP, IPAddress.Loopback); //should be true 127.0.0.1
			Assert.AreNotEqual(details.RemoteIP, IPAddress.Any);
			Assert.AreEqual(600, details.RemotePort); //check that the ports are equal
		}
	}
}
