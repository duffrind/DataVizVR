using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WMSInfoRequester : ServerInfoRequester<WMSInfo> {
	protected override WMSInfo ParseResponse( string responseText )
	{
		return WMSXMLParser.GetWMSInfo (responseText);
	}


	protected override string BuildQueryString()
	{
		return "?REQUEST=GetCapabilities&SERVICE=WMS&VERSION=1.1.0";
	}
}
