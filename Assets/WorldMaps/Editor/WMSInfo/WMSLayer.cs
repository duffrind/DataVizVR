using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public class WMSLayer
{
	public string title;
	public string name;
	public List<WMSBoundingBox> boundingBoxes;
	public WMSLayer parentLayer;

	public List<WMSBoundingBox> GetBoundingBoxes()
	{
		List<WMSBoundingBox> boundingBoxes = new List<WMSBoundingBox> ();

		if( parentLayer != null ){
			boundingBoxes.AddRange ( parentLayer.GetBoundingBoxes() );
		}
		boundingBoxes.AddRange ( this.boundingBoxes );

		return boundingBoxes;
	}


	public WMSBoundingBox GetBoundingBox( int index )
	{
		WMSBoundingBox[] boundingBoxes = GetBoundingBoxes().ToArray();

		return boundingBoxes[index];
	}


	public string[] GetBoundingBoxesNames()
	{
		WMSBoundingBox[] boundingBoxes = GetBoundingBoxes().ToArray ();
		string[] boundingBoxesNames = new string[boundingBoxes.Length];

		for (int i=0; i<boundingBoxes.Length; i++) {
			boundingBoxesNames[i] = boundingBoxes[i].ToString ();
		}

		return boundingBoxesNames;
	}
}