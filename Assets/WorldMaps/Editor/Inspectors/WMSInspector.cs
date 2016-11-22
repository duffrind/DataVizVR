using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.IO;
using System.Linq;

[CustomEditor(typeof(WMSTexture))]
public class WMSComponentInspector : Editor
{
	private static WMSServerBookmarks bookmarks = new WMSServerBookmarks();
	private static WMSInfoRequester wmsInfoRequester = new WMSInfoRequester();


	public override void OnInspectorGUI()
	{
		if (Application.isPlaying) {
			EditorGUILayout.LabelField ("Currently play mode editting is not allowed");
			return;
		}

		WMSTexture wmsTexture = (WMSTexture)target;

		bool serverChanged = false;
		bool layerChanged = false;
		bool boundingBoxChanged = false;

		EditorGUILayout.BeginVertical (EditorStyles.helpBox);
			EditorGUILayout.LabelField ("Server");
			DisplayServerSelector (ref wmsTexture, out serverChanged);

			if (serverChanged) {
				wmsTexture.selectedLayers.Clear ();
				RequestWMSInfo (ref wmsTexture);
			}

			RequestStatus requestStatus = 
				wmsInfoRequester.GetRequestStatus (wmsTexture.wmsRequestID);
				
			if (requestStatus != RequestStatus.OK) {
				if( requestStatus == RequestStatus.DOWNLOADING ){
					EditorGUILayout.HelpBox("Downloading WMS info ...", MessageType.Info);
				}else if( requestStatus == RequestStatus.ERROR ){
					string errorLog = wmsInfoRequester.GetErrorLog (wmsTexture.wmsRequestID);
					EditorGUILayout.HelpBox("ERROR downloading WMS info: \n" + errorLog, MessageType.Error);
				}

				if (GUI.changed) {
					EditorUtility.SetDirty (wmsTexture);
					EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
				}
				EditorGUILayout.EndVertical ();
				return;
			}

			WMSInfo wmsInfo = wmsInfoRequester.GetResponse (wmsTexture.wmsRequestID);
			EditorGUILayout.LabelField ("Server title: " + wmsInfo.serverTitle);
			EditorGUILayout.LabelField ("Server abstract: " + wmsInfo.serverAbstract);
		EditorGUILayout.EndVertical ();

		if (wmsInfo.GetLayerTitles ().Length > 0) {
			DisplayLayersSelector (ref wmsTexture, wmsInfo, out layerChanged);

			if (layerChanged) {
				wmsTexture.RequestTexturePreview ();
			}

			if (wmsTexture.SelectedLayersNumber () > 0) {
				DisplayBoundingBoxPanel (ref wmsTexture, ref wmsInfo, out boundingBoxChanged);
			}
		} else {
			EditorGUILayout.HelpBox("No layers returned by server", MessageType.Warning);
		}
			

		if (wmsTexture.IsDownloading ()) {
			EditorGUILayout.HelpBox("Downloading texture from server...", MessageType.Info);
		}


		// Mark the target assert as changed ("dirty") so Unity save it to disk.
		if (GUI.changed) {
			EditorUtility.SetDirty (wmsTexture);
			EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
		}
	}


	private void RequestWMSInfo(ref WMSTexture wmsTexture)
	{
		wmsTexture.wmsRequestID = wmsInfoRequester.RequestServerInfo (wmsTexture.serverURL);
		wmsTexture.RequestTexturePreview ();
		EditorApplication.update += Refresh;
	}


	private void DisplayServerSelector(ref WMSTexture wmsTexture, out bool serverChanged)
	{
		serverChanged = false;

		DisplayServerPopup (ref wmsTexture, ref serverChanged);

		string newServerURL = EditorGUILayout.TextField("Server URL:", wmsTexture.serverURL);

		serverChanged |= (newServerURL != wmsTexture.serverURL);
		wmsTexture.serverURL = newServerURL;

		if ( (wmsTexture.wmsRequestID != "") && (wmsInfoRequester.GetRequestStatus(wmsTexture.wmsRequestID) == RequestStatus.OK)){
			WMSInfo wmsInfo = 
				wmsInfoRequester.GetResponse (wmsTexture.wmsRequestID);
			
			if (!bookmarks.ServerIsBookmarked (wmsInfo.serverTitle)) {
				DisplayServerBookmarkButton (wmsInfo.serverTitle, wmsTexture.serverURL);
			//} else {
			//	RemoveServerFromBookmarksButton (requestStatus.response.serverTitle);
			}
		}
	}


	private void DisplayServerPopup(ref WMSTexture wmsTexture, ref bool serverChanged)
	{
		string[] serverTitles = bookmarks.ServerTitles();

		int newServerIndex = EditorGUILayout.Popup ("Bookmarked servers", 0, serverTitles);
		serverChanged = (newServerIndex != 0 && bookmarks.GetServerURL(serverTitles[newServerIndex]) != wmsTexture.serverURL);

		if (serverChanged) {
			wmsTexture.serverURL = bookmarks.GetServerURL (serverTitles[newServerIndex]);
		}
	}


	private void DisplayLayersSelector( ref WMSTexture wmsTexture, WMSInfo wmsInfo, out bool layerChanged )
	{
		layerChanged = false;

		EditorGUILayout.BeginVertical (EditorStyles.helpBox);
		EditorGUILayout.LabelField ("Layers");

		WMSLayer[] layers = wmsInfo.layers;
		for( int i=0; i<layers.Length; i++) {
			if( layers[i].name != "" ){
				bool newToggleValue = 
					EditorGUILayout.ToggleLeft (layers[i].title, (wmsTexture.LayerSelected(layers[i].name)));
				
				layerChanged |= (newToggleValue != wmsTexture.LayerSelected(layers[i].name));
				wmsTexture.SetLayerSelected (layers[i].name, newToggleValue);
			}
		}

		if (wmsTexture.SelectedLayersNumber () == 0) {
			EditorGUILayout.HelpBox ("No layers selected", MessageType.Warning);
		}

		EditorGUILayout.EndVertical ();
	}


	public void DisplayBoundingBoxPanel(ref WMSTexture wmsTexture, ref WMSInfo wmsInfo, out bool boundingBoxChanged)
	{
		EditorGUILayout.BeginVertical (EditorStyles.helpBox);

		EditorGUILayout.LabelField ("Bounding Box");

		DisplayBoundingBoxSelector (ref wmsTexture, wmsInfo, out boundingBoxChanged);

		EditorGUILayout.LabelField ("SRS", wmsTexture.SRS);

		wmsTexture.keepBoundingBoxRatio = 
			EditorGUILayout.Toggle ("Keep ratio", wmsTexture.keepBoundingBoxRatio);

		Vector2 newBottomLeftCoordinates =
			EditorGUILayout.Vector2Field (
				"Bottom left coords.",
				wmsTexture.bottomLeftCoordinates
			);

		Vector2 newTopRightCoordinates =
			EditorGUILayout.Vector2Field (
				"Top right coords.",
				wmsTexture.topRightCoordinates
			);

		UpdateBoundingBox (
			ref wmsTexture.bottomLeftCoordinates,
			ref wmsTexture.topRightCoordinates,
			newBottomLeftCoordinates,
			newTopRightCoordinates,
			wmsTexture.keepBoundingBoxRatio);

		if(GUILayout.Button("Update bounding box preview (may take a while)")){
			wmsTexture.RequestTexturePreview ();
		}

		EditorGUILayout.EndVertical ();
	}


	private void DisplayBoundingBoxSelector( ref WMSTexture wmsTexture, WMSInfo wmsInfo, out bool boundingBoxChanged )
	{
		boundingBoxChanged = false;

		List<string> boundingBoxesNames = wmsInfo.GetBoundingBoxesNames(wmsTexture.selectedLayers).ToList();
		boundingBoxesNames.Insert (0, "Select bounding box from server");

		if( boundingBoxesNames.Count > 1 ){
			int newBoundingBoxIndex = 
				EditorGUILayout.Popup (
					"BBs from server",
					0, 
					boundingBoxesNames.ToArray()
					) - 1;
			
			boundingBoxChanged = (newBoundingBoxIndex != -1);

			if( boundingBoxChanged ){
				wmsTexture.SRS = wmsInfo.GetBoundingBox (wmsTexture.selectedLayers, newBoundingBoxIndex).SRS;
				WMSBoundingBox currentBoundingBox = wmsInfo.GetBoundingBox( wmsTexture.selectedLayers, newBoundingBoxIndex );

				wmsTexture.bottomLeftCoordinates = currentBoundingBox.bottomLeftCoordinates;
				wmsTexture.topRightCoordinates = currentBoundingBox.topRightCoordinates;

				wmsTexture.RequestTexturePreview ();
			}else{
				if( wmsTexture.selectedLayers.Count == 1 ){
					// If we have one layer selected, use the SRS of its first bounding box.
					wmsTexture.SRS = wmsInfo.GetBoundingBox (wmsTexture.selectedLayers, 0).SRS;
				}
			}
		}
	}


	private void UpdateBoundingBox( 
	                               ref Vector2 oldBottomLeftCoordinates, 
	                               ref Vector2 oldTopRightCoordinates,
	                               Vector2 newBottomLeftCoordinates, 
	                               Vector2 newTopRightCoordinates,
								   bool keepRatio)
	{
		Vector2 auxBottomLeftCoordinates = newBottomLeftCoordinates;
		Vector2 auxTopRightCoordinates = newTopRightCoordinates;

		if (keepRatio) {
			// Compute aspect ratio
			float width = oldTopRightCoordinates.y - oldBottomLeftCoordinates.y;
			if (width == 0.0f) {
				width = 1.0f;
			}

			float height = oldTopRightCoordinates.y - oldBottomLeftCoordinates.y;
			if (height == 0.0f) {
				height = 1.0f;
			}

			float boundigBoxRatio = width / height;
			
			// Update Y coordinates according to bounding box ratio (if X changed).
			auxBottomLeftCoordinates.y += (newBottomLeftCoordinates.x - oldBottomLeftCoordinates.x) / boundigBoxRatio;
			auxTopRightCoordinates.y += (newTopRightCoordinates.x - oldTopRightCoordinates.x) / boundigBoxRatio;
			
			// Update X coordinates according to bounding box ratio (if Y changed).
			auxBottomLeftCoordinates.x += (newBottomLeftCoordinates.y - oldBottomLeftCoordinates.y) * boundigBoxRatio;
			auxTopRightCoordinates.x += (newTopRightCoordinates.y - oldTopRightCoordinates.y) * boundigBoxRatio;
		}
		
		oldBottomLeftCoordinates = auxBottomLeftCoordinates;
		oldTopRightCoordinates = auxTopRightCoordinates;
	}


	private void DisplayServerBookmarkButton(string serverTitle, string serverURL)
	{
		if (GUILayout.Button ("Bookmark server")){
			bookmarks.BookmarkServer (serverTitle, serverURL);
		}
	}


	private void DisplayRemoveServerFromBookmarksButton(string serverTitle)
	{
		if (GUILayout.Button ("Remove server from bookmarks")){
			bookmarks.RemoveServerFromBookmarks (serverTitle);
		}
	}
		

	public void OnEnable()
	{
		WMSTexture wmsTexture = (WMSTexture)target;
		if (!Application.isPlaying && (wmsTexture.wmsRequestID == "" || !wmsInfoRequester.ExistsTransaction (wmsTexture.wmsRequestID))) {
			RequestWMSInfo (ref wmsTexture);
		}
	}


	public void OnDisable()
	{
		EditorApplication.update -= Refresh;
	}


	public void Refresh()
	{
		WMSTexture wmsTexture = (WMSTexture)target;
		if (wmsInfoRequester.Update (wmsTexture.wmsRequestID) != RequestStatus.DOWNLOADING) {
			// Stop refreshing when server download stops.
			EditorApplication.update -= Refresh;
			Repaint ();
		}
	}
}
