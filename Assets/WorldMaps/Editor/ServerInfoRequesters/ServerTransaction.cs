using UnityEngine;
using System;
using System.Collections;
using System.IO;


public enum RequestStatus
{
	DOWNLOADING,
	OK,
	ERROR
}


class ServerTransaction <ResponseType>
	where ResponseType : class
{
	public delegate ResponseType ParsingFunction(string responseText);

	public WWW request = null;
	public ResponseType response = null;
	public string errorLog = null;


	public ServerTransaction(string url, ParsingFunction parsingFunction)
	{
		if (RequestIsCached (url)) {
			ParseResponse (File.ReadAllText (URLToFilePath (url)), parsingFunction);
		} else {
			request = new WWW (url);
		}
	}


	public RequestStatus Update( ParsingFunction ParseResponse )
	{
		RequestStatus requestStatus = GetRequestStatus ();

		if (requestStatus == RequestStatus.DOWNLOADING) {
			if (request.isDone) {
				if (request.error == null) {
					if (this.ParseResponse (request.text, ParseResponse)) {
						// Cache the result
						File.WriteAllText (URLToFilePath (request.url), request.text);
						return RequestStatus.OK;
					} else {
						return RequestStatus.ERROR;
					}
				} else {
					// Connection error
					this.errorLog = "Couldn't get info from server: " + request.error;
					return RequestStatus.ERROR;
				}
			}
			return RequestStatus.DOWNLOADING;
		} else {
			// Status other than "downloading" remain the same when updating.
			return requestStatus;
		}
	}


	public RequestStatus GetRequestStatus()
	{
		if (response != null) {
			return RequestStatus.OK;
		} else if (errorLog != null) {
			return RequestStatus.ERROR;
		} else {
			return RequestStatus.DOWNLOADING;
		}
	}


	public bool ParseResponse( string responseText, ParsingFunction parsingFunction )
	{
		try{
			response = parsingFunction (responseText);
			return true;
		}catch( Exception e ){
			// Parsing error
			this.errorLog = "Couldn't parse response from server: " + e.Message + "\nResponse from server: \n" + request.text;
			return false;
		}
	}


	public static string URLToFilePath(string url)
	{
		string filepath = url;

		filepath = filepath.Replace ('/', '-');
		filepath = filepath.Replace ('.', '-');
		filepath = filepath.Replace ('\\', '-');
		filepath = filepath.Replace (':', '-');
		filepath = filepath.Replace ('?', '-');

		return "Temp/" + filepath;
	}


	public static bool RequestIsCached(string url)
	{
		string filepath = URLToFilePath(url);

		// TODO: Check download date.
		return File.Exists (filepath);
	}
}