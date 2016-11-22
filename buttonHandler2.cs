using UnityEngine;
using System.Collections;
using Vuforia;
using System.Collections.Generic;

public class buttonHandler : MonoBehaviour, IVirtualButtonEventHandler {
	//public TextMesh Button1;
	private TextMesh textObject;
	public TextAsset csv;
	private string[,] output; 
	private GameObject cb;
	private bool active;
	// Use this for initialization
	void Start () {
		active = true;
		CSVReader.DebugOutputGrid( CSVReader.SplitCsvGrid(csv.text) );
		output = CSVReader.SplitCsvGrid (csv.text);
		// Search for all Children from this ImageTarget with type VirtualButtonBehaviour
		VirtualButtonBehaviour[] vbs = GetComponentsInChildren<VirtualButtonBehaviour>();
		for (int i = 0; i < vbs.Length; ++i) { vbs[i].RegisterEventHandler(this); }

		textObject = GameObject.Find ("Button2").GetComponent<TextMesh>();
		cb = GameObject.Find ("height");//.GetComponent<Plane>();
		//cb = false;
		//textObject.color = Color.black;
		//t = (TextMesh)gameObject.GetComponent(typeof(TextMesh));
		//temper = GetComponent<Button1>();
		//t = transform.GetComponent<TextMesh>();//.FindChild("Button1").gameObject;
		//Button2 = transform.FindChild("Button2").gameObject;
		//Button3 = transform.FindChild("Button3").gameObject;
	}

	// Update is called once per frame
	//void Update () {
	
	//}

	public void OnButtonPressed(VirtualButtonAbstractBehaviour vb) { 
		
		switch(vb.VirtualButtonName) {
		case "b1":
			//textObject.color = Color.red;
			//print (output);
			//t.GetComponent(TextMesh).gameObject.color = Color.red;//Button1.GetComponent<Renderer>().material.color = Color.red;
			break;
		case "b2":
			textObject.color = Color.red;
			//cb.enabled = true;
			active = !active;
			cb.SetActive(active);
			break;
		case "b3":
			//do something
			break;
		default:
			throw new UnityException("Button not supported: " + vb.VirtualButtonName);
			//break;
		}
	}

	/// <summary>
	/// Called when the virtual button has just been released:
	/// </summary>
	public void OnButtonReleased(VirtualButtonAbstractBehaviour vb) {
		if (vb.VirtualButtonName=="b2")  { textObject.color = Color.white; }
	}
}