using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class WMSServerBookmarks {
	// [title] -> [url]
	private Dictionary<string, string> servers_ = 
		new Dictionary<string, string>();
	private string[] keyValueSeparator_ = new string[]{" @*-*@ "};


	public WMSServerBookmarks()
	{
		string[] lines = File.ReadAllLines ("Assets/WorldMaps/WMSServerBookmarks");

		foreach (string line in lines) {
			string serverTitle = line.Split (keyValueSeparator_, 2, System.StringSplitOptions.None) [0];
			string serverURL = line.Split (keyValueSeparator_, 2, System.StringSplitOptions.None) [1];
			servers_ [serverTitle] = serverURL;
		}
	}


	~WMSServerBookmarks(){
		string[] lines = new string[servers_.Count];
		int i = 0;
		foreach(KeyValuePair<string, string> entry in servers_){
			lines [i] = entry.Key + keyValueSeparator_[0] + entry.Value;
			i++;
		}

		File.WriteAllLines("Assets/WorldMaps/WMSServerBookmarks", lines);
	}


	public void BookmarkServer(string serverTitle, string serverURL)
	{
		servers_ [serverTitle] = serverURL;
	}


	public void RemoveServerFromBookmarks(string serverTitle)
	{
		servers_.Remove (serverTitle);
	}


	public bool ServerIsBookmarked(string serverTitle){
		return servers_.ContainsKey(serverTitle);
	}


	public string[] ServerTitles()
	{
		string[] lines = new string[servers_.Count];
		int i = 0;
		foreach(KeyValuePair<string, string> entry in servers_){
			lines [i] = entry.Key;
			i++;
		}
		return lines;
	}


	public string GetServerURL(string serverTitle)
	{
		return servers_ [serverTitle];
	}
}
