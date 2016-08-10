using GladNet.ASP.Client.RestSharp.Middleware.Authentication;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.ASP.Client.RestSharp.Tests
{
	[TestFixture]
	public static class JWTModelTests
	{
		[Test]
		public static void Test_Ctor_Doesnt_Throw()
		{
			//assert
			Assert.DoesNotThrow(() => new JWTModel());
		}

		//We test this because fields should be optional. An invalid response will show up as an invalid token to consumers.
		[Test]
		public static void Test_Can_Deserialize_From_Empty_String()
		{
			//assert
			Assert.DoesNotThrow(() => JsonConvert.DeserializeObject<JWTModel>(""));
		}
	}
}
