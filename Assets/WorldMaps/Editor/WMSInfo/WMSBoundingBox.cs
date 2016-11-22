using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class WMSBoundingBox
{
	public string SRS;
	public Vector2 bottomLeftCoordinates;
	public Vector2 topRightCoordinates;


	public override string ToString ()
	{
		return bottomLeftCoordinates + ", " + topRightCoordinates;
	}


	public float ratio()
	{
		return (topRightCoordinates.x - bottomLeftCoordinates.x) / (topRightCoordinates.y - bottomLeftCoordinates.y);
	}
}