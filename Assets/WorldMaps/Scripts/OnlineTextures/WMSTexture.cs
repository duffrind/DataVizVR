using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;


public class WMSTexture : OnlineTexture {
	public string serverURL = "http://129.206.228.72/cached/osm";
	public string wmsRequestID = "";
	public bool keepBoundingBoxRatio = false;
	public Vector2 bottomLeftCoordinates = new Vector2 ( 416000,3067000 );
	public Vector2 topRightCoordinates = new Vector2 ( 466000,3117000 );
	public List<string> selectedLayers = new List<string>();
	public string wmsVersion = "1.1.0";
	public string SRS = "";


	protected override string GenerateRequestURL (string nodeID)
	{
		Vector2 bottomLeftCoordinates = this.bottomLeftCoordinates;
		Vector2 topRightCoordinates = this.topRightCoordinates;
		GenerateWMSBoundingBox (nodeID, ref bottomLeftCoordinates, ref topRightCoordinates);
			
		string fixedUrl = serverURL + BuildWMSFixedQueryString();
		string bboxUrlQuery = 
			"&BBOX=" + bottomLeftCoordinates.x.ToString("F") + "," +
			bottomLeftCoordinates.y.ToString("F") + "," +
			topRightCoordinates.x.ToString("F") + "," +
			topRightCoordinates.y.ToString("F");

		return fixedUrl + bboxUrlQuery;
	}


	public void SetLayerSelected( string layerName, bool layerSelected ){
		if (layerSelected && !selectedLayers.Contains (layerName)) {
			selectedLayers.Add (layerName);
		} else if (!layerSelected && selectedLayers.Contains (layerName)) {
			selectedLayers.Remove (layerName);
		}
	}


	public bool LayerSelected( string layerName )
	{
		return selectedLayers.Contains (layerName);
	}


	public int SelectedLayersNumber()
	{
		return selectedLayers.Count;
	}


	private string BuildWMSFixedQueryString()
	{
		string layersQuery = "";
		string stylesQuery = "";
		foreach (string layerName in selectedLayers) {
			layersQuery += layerName + ",";
			stylesQuery += ",";
		}
		// Remove last character (',').
		if (layersQuery.Length > 0) {
			layersQuery = layersQuery.Remove(layersQuery.Length - 1);
			stylesQuery = stylesQuery.Remove(stylesQuery.Length - 1);
		}
		return 
			"?SERVICE=WMS" +
			"&LAYERS=" + layersQuery +
			"&REQUEST=GetMap&VERSION=" + wmsVersion +
			"&FORMAT=image/jpeg" +
			"&SRS=" + SRS +
			"&STYLES=" + stylesQuery +
			"&WIDTH=128&HEIGHT=128&REFERER=CAPAWARE";
	}


	private void GenerateWMSBoundingBox(string nodeID, ref Vector2 bottomLeftCoordinates, ref Vector2 topRightCoordinates)
	{
		for (int i = 1; i < nodeID.Length; i++) {
			float x0 = bottomLeftCoordinates.x;
			float y0 = bottomLeftCoordinates.y;
			float x1 = topRightCoordinates.x;
			float y1 = topRightCoordinates.y;
			float cx = (x0 + x1)/2.0f;
			float cy = (y0 + y1)/2.0f;

			if (nodeID [i] == '0') {
				bottomLeftCoordinates = new Vector2( x0, cy );
				topRightCoordinates = new Vector2 (cx, y1);
			}else if(nodeID[i] == '1'){
				bottomLeftCoordinates = new Vector2( x0, y0 );
				topRightCoordinates = new Vector2( cx, cy );
			}else if(nodeID[i] == '2'){
				bottomLeftCoordinates = new Vector2( cx, cy );
				topRightCoordinates = new Vector2( x1, y1 );
			}else if(nodeID[i] == '3'){
				bottomLeftCoordinates = new Vector2( cx, y0 );
				topRightCoordinates = new Vector2( x1, cy );
			}
		}
	}


	protected override void InnerCopyTo(OnlineTexture copy)
	{
		WMSTexture target = (WMSTexture)copy;
		target.serverURL = serverURL;
		target.wmsRequestID = wmsRequestID;
		target.keepBoundingBoxRatio = keepBoundingBoxRatio;
		target.bottomLeftCoordinates = bottomLeftCoordinates;
		target.topRightCoordinates = topRightCoordinates;
		target.selectedLayers = selectedLayers;
		target.wmsVersion = wmsVersion;
		target.SRS = SRS;
	}


	public override bool ValidateDownloadedTexture( out string errorMessage )
	{
		if (request_.text.IndexOf("<ServiceException>") != -1) {
			errorMessage = 
				"Service exception error returned by server: \n" + request_.text;
			return false;
		}else{
			return base.ValidateDownloadedTexture(out errorMessage);
		}
	}
}
