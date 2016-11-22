using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour {

	public Color[] colors;

	public HexGrid hexGrid;

	//private CSVReader file;
	public TextAsset csv;
	private string[,] output;

	int activeElevation;

	private HexCell[] cell;

	Color activeColor;

	public void SelectColor (int index) {
		activeColor = colors[index];
	}

	public void SetElevation (float elevation) {
		activeElevation = (int)elevation;
	}

	void Awake () {
		SelectColor(0);
		output = CSVReader.SplitCsvGrid (csv.text);
		//for () //int.parse(string)
		cell = hexGrid.GetCells();
		//file[0]
		for (int i = 0; i < 80; i++) {
			int elev = (int)Mathf.Log10 (float.Parse (output [i % 10, i / 10]) + 1);
			cell[i].Elevation = elev;
			if (elev <= 1) {
				cell [i].color = new Color(0F,1F,0F,0.1F);
			}
			else if (elev <= 2) {
				cell [i].color = new Color(1F, 0.92F, 0.016F, 0.1F);
			}
			else {
				cell [i].color = new Color(1F, 0F, 0F, 0.1F);
			}
		}

		//cell[0].Elevation = (int)Mathf.Log(6, 2);
		hexGrid.Refresh();
		// Let's set the elevations to the (log of) text file and if high = red, med = yellow, low = green
	}

	void Update () {
		if (
			Input.GetMouseButton(0) &&
			!EventSystem.current.IsPointerOverGameObject()
		) {
			HandleInput();
		}
	}

	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			EditCell(hexGrid.GetCell(hit.point));
		}
	}

	void EditCell (HexCell cell) {
		cell.color = activeColor;
		cell.Elevation = activeElevation;
		hexGrid.Refresh();
	}
}