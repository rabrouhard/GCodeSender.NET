using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GCodeSender.NET
{
	static class Util
	{
		public static IPEndPoint ParseIPEndPoint(string text)
		{
			Uri uri;
			if (Uri.TryCreate(text, UriKind.Absolute, out uri))
				return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);
			if (Uri.TryCreate(String.Concat("tcp://", text), UriKind.Absolute, out uri))
				return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);
			if (Uri.TryCreate(String.Concat("tcp://", String.Concat("[", text, "]")), UriKind.Absolute, out uri))
				return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);
			throw new Exception("Failed to parse text to IPEndPoint");
		}
	}
}
