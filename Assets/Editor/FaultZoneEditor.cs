using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Faultzone))]
public class FaultZoneEditor : Editor{

	private Faultzone myScript;
	
	int index = 0;
	string[] options = new string[]{ "Sinkhole", "Tremor", "Rise" };

	// Use this for initialization
	void Start () 
	{

	}

	public void OnEnable()
	{
		myScript = (Faultzone)target;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}


	public override void OnInspectorGUI()
	{
		index = myScript.GetTypeAsInt();
		index = EditorGUILayout.Popup("Fault Type:", index, options, EditorStyles.popup);
		switch(index)
		{
			//Sinkhole
		case 0:
			myScript.type = EarthquakeManager.TYPES.SINKHOLE;
			DrawSinkhole();
			break;
			//Tremor
		case 1:
			myScript.type = EarthquakeManager.TYPES.TREMOR;
			DrawTremor();
			break;
			//Rise
		case 2:
			myScript.type = EarthquakeManager.TYPES.RISE;
			DrawRise();
			break;
		}

		EditorGUILayout.LabelField("General Settings:");
		myScript.durationMin = EditorGUILayout.FloatField("Duration Min: ", myScript.durationMin);
		myScript.durationMax = EditorGUILayout.FloatField("Duration Max: ", myScript.durationMax);
		myScript.delayMin = EditorGUILayout.FloatField("Delay Min: ", myScript.delayMin);
		myScript.delayMax = EditorGUILayout.FloatField("Delay Max: ", myScript.delayMax);
		myScript.cutoffDistance = EditorGUILayout.FloatField("Cutoff Distance: ", myScript.cutoffDistance);
		myScript.size = EditorGUILayout.FloatField("Gizmo radius: ", myScript.size);
		myScript.colour = EditorGUILayout.ColorField("Gizmo Colour:",Color.red);
	}

	private void DrawTremor()
	{
		EditorGUILayout.LabelField("Tremor Settings:");
		myScript.widthMin = EditorGUILayout.IntField("Crack Width Min: ", myScript.widthMin);
		myScript.widthMax = EditorGUILayout.IntField("Crack Width Max: ", myScript.widthMax);
		myScript.heightMin = EditorGUILayout.IntField("Depth Min: ", myScript.heightMin);
		myScript.heightMax = EditorGUILayout.IntField("Depth Max: ", myScript.heightMax);
		myScript.lengthMin = EditorGUILayout.FloatField("Crack Length Min: ", myScript.lengthMin);
		myScript.lengthMax = EditorGUILayout.FloatField("Crack Length Max: ", myScript.lengthMax);

	}

	private void DrawSinkhole()
	{
		EditorGUILayout.LabelField("Sinkhole Settings:");
		myScript.heightMin = EditorGUILayout.IntField("Sinkhole Depth Min: ", myScript.heightMin);
		myScript.heightMax = EditorGUILayout.IntField("Sinkhole Depth Max: ", myScript.heightMax);
		myScript.areaOfEffect = EditorGUILayout.FloatField("Sinkhole Size: ", myScript.areaOfEffect);
	}

	private void DrawRise()
	{
		EditorGUILayout.LabelField("Rise Settings:");
		myScript.widthMin = EditorGUILayout.IntField("Rise Width Min: ", myScript.widthMin);
		myScript.widthMax = EditorGUILayout.IntField("Rise Width Max: ", myScript.widthMax);
		myScript.lengthMin = EditorGUILayout.FloatField("Rise Length Min: ", myScript.lengthMin);
		myScript.lengthMax = EditorGUILayout.FloatField("Rise Length Max: ", myScript.lengthMax);
		myScript.heightMin = EditorGUILayout.IntField("Height Min: ", myScript.heightMin);
		myScript.heightMax = EditorGUILayout.IntField("Height Max: ", myScript.heightMax);

	}

}
