using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class WMSInfo
{
	[SerializeField]
	public string serverTitle;
	[SerializeField]
	public string serverAbstract;
	[SerializeField]
	public WMSLayer[] layers = new WMSLayer[0]{};
	[SerializeField]
	private int currentLayerIndex = 0;


	public WMSInfo( string serverTitle, string serverAbstract, WMSLayer[] layers )
	{
		this.serverTitle = serverTitle;
		this.serverAbstract = serverAbstract;
		this.layers = layers;
	}


	public string[] GetLayerTitles()
	{
		string[] layerTitles = new string[layers.Length];
		
		for (int i=0; i<layers.Length; i++) {
			layerTitles[i] = layers[i].title;
		}

		return layerTitles;
	}


	public int CurrentLayerIndex()
	{
		return currentLayerIndex;
	}


	public WMSLayer GetLayer( int index )
	{
		return layers [index];
	}


	public List<WMSBoundingBox> GetBoundingBoxes(List<string> selectedLayers)
	{
		List<WMSBoundingBox> boundingBoxes = new List<WMSBoundingBox> ();

		foreach (WMSLayer layer in layers) {
			if( selectedLayers.Contains(layer.name) ){
				boundingBoxes.AddRange (layer.GetBoundingBoxes());
			}
		}

		return boundingBoxes;
	}


	public string[] GetBoundingBoxesNames(List<string> selectedLayers)
	{
		WMSBoundingBox[] boundingBoxes = GetBoundingBoxes(selectedLayers).ToArray ();
		string[] boundingBoxesNames = new string[boundingBoxes.Length];
		
		for (int i=0; i<boundingBoxes.Length; i++) {
			boundingBoxesNames[i] = boundingBoxes[i].ToString ();
		}
		
		return boundingBoxesNames;
	}


	public WMSBoundingBox GetBoundingBox(List<string> selectedLayers, int index)
	{
		return GetBoundingBoxes(selectedLayers).ToArray ()[index];
	}

}