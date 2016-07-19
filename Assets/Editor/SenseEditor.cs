using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Sense))]
public class SenseEditor : Editor {

	private Sense myScript;
	private string[] options = new string[]{"Hearing","Sight"};
	private int index =0;
	// Use this for initialization
	void Start () 
	{
	
	}

	void OnEnable()
	{
		myScript = (Sense)target;
	}

	// Update is called once per frame
	void Update () {
	
	}

	public override void OnInspectorGUI()
	{
		index = myScript.GetSenseTypeAsInt();
		index = EditorGUILayout.Popup("Sense Type:", index, options, EditorStyles.popup);
		switch(index)
		{
			//hearing
		case 0:
			myScript.sense = Sense.SENSE_TYPE.HEARING;
			myScript.hearingSensor = (SphereCollider)EditorGUILayout.ObjectField("Hearing Sphere Collider: ",myScript.hearingSensor,typeof(SphereCollider));
			break;
			//sight
		case 1:
			myScript.sense = Sense.SENSE_TYPE.SIGHT;
			myScript.sightSensor = (BoxCollider)EditorGUILayout.ObjectField("Sight Box Collider: ",myScript.sightSensor,typeof(BoxCollider));
			break;
		}

		//General settings
		EditorGUILayout.LabelField("General Settings:");
		myScript.equippedTo = (GameObject)EditorGUILayout.ObjectField("Equipped to:",myScript.equippedTo,typeof(GameObject));
		EditorGUILayout.LabelField("These modifiers are based off the original size of the sensor");
		myScript.alertMod = EditorGUILayout.FloatField("Alert Multiplier:",myScript.alertMod);
		myScript.combatMod = EditorGUILayout.FloatField("Combat Multiplier:",myScript.combatMod);
		myScript.changeSpeed = EditorGUILayout.FloatField("Sense Change Speed:", myScript.changeSpeed);
		myScript.playerTag = EditorGUILayout.TextField("Player tag:",myScript.playerTag);
		myScript.obstacleTag = EditorGUILayout.TextField("Obstacle tag:",myScript.obstacleTag);

	}
}
