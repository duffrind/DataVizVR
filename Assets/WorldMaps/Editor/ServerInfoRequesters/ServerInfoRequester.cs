using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public abstract class ServerInfoRequester <ResponseType>
	where ResponseType : class
{
	Dictionary<string, ServerTransaction<ResponseType>> transactions = 
		new Dictionary<string, ServerTransaction<ResponseType>>();


	public string RequestServerInfo( string serverURL )
	{
		string requestID = GenerateRequestID (serverURL);
		if (!transactions.ContainsKey (requestID) || transactions[requestID].errorLog != null){
			transactions [requestID] = new ServerTransaction<ResponseType> (serverURL + BuildQueryString(), ParseResponse);
		}
		return requestID;
	}


	public bool ExistsTransaction(string requestID)
	{
		return transactions.ContainsKey (requestID);
	}


	public RequestStatus Update( string requestID )
	{
		return transactions [requestID].Update (ParseResponse);
	}


	public RequestStatus GetRequestStatus (string requestID)
	{
		return transactions [requestID].GetRequestStatus();
	}


	public ResponseType GetResponse( string requestID )
	{
		return transactions [requestID].response;
	}


	public string GetErrorLog( string requestID )
	{
		return transactions [requestID].errorLog;
	}
		

	protected string GenerateRequestID( string serverURL )
	{
		return serverURL;
	}


	protected abstract ResponseType ParseResponse( string responseText );
	protected abstract string BuildQueryString();
}
