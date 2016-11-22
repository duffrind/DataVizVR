using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(BingMapsTexture))]
public class BingMapsInspector : Editor
{
	const int MIN_ZOOM = 0;
	const int MAX_ZOOM = 7;

	static string lattitudeLabel = "Lattitude (decimal): ";
	static string longitudeLabel = "Longitude (decimal): ";
	static string zoomLabel = "Zoom: ";


	public override void OnInspectorGUI()
	{
		if (Application.isPlaying) {
			EditorGUILayout.LabelField ("Currently play mode editting is not allowed");
			return;
		}

		BingMapsTexture bingMapsTexture = (BingMapsTexture)target;

		EditorGUILayout.LabelField ("Server template URL");
		bingMapsTexture.serverURL = EditorGUILayout.TextField (bingMapsTexture.serverURL);
		if (bingMapsTexture.serverURL == BingMapsTexture.testServerURL) {
			EditorGUILayout.HelpBox("This is a test server URL. When building your app, please generate a new server template URL by following the instructions on file Assets/WorldMaps/README.pdf", MessageType.Warning);
		}

		string errorMessage = "";
		if( BingMapsTexture.ValidateServerURL(bingMapsTexture.serverURL, out errorMessage) == false ){
			EditorGUILayout.HelpBox (errorMessage, MessageType.Error);
		}

		bingMapsTexture.latitude = EditorGUILayout.FloatField(lattitudeLabel, bingMapsTexture.latitude);
		bingMapsTexture.longitude = EditorGUILayout.FloatField(longitudeLabel, bingMapsTexture.longitude);
		bingMapsTexture.initialZoom = EditorGUILayout.IntSlider(zoomLabel, bingMapsTexture.initialZoom, MIN_ZOOM, MAX_ZOOM);
		bingMapsTexture.ComputeInitialSector ();

		if (GUILayout.Button ("Update preview (may take a while)")) {
			bingMapsTexture.RequestTexturePreview ();
		}

		if (bingMapsTexture.IsDownloading ()) {
			EditorGUILayout.HelpBox("Downloading texture from server...", MessageType.Info);
		}

		if (GUI.changed) {
			EditorUtility.SetDirty (bingMapsTexture);
			EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
		}
	}
}
