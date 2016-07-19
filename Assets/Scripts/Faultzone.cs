using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Faultzone : MonoBehaviour {

	//Gizmos stuff
	public Color colour = Color.red;
	public float size = 1f;

	//Fault Zone code
	//The fault zone is going to be the point where the earthquake actually effects the world
	//The settings here will be things like, its area of effect, the magnitude cutoff,
	//Number of cracks, length of cracks, width of cracks and depth of the cracks
	//These define areas where the earthquake will affect the terrain and the values that are used

	//The EarthquakeManager handles the start and end of the quakes and the terrain deformation and such

	//Faultzone variables
	public EarthquakeManager.TYPES type;
	public float delayMin = 10.0f; //Delay minimum
	public float delayMax = 20.0f; //Delay Max
	//Width of crack and width of a rise
	public int widthMin = 1; // Width Min
	public int widthMax = 5; // Width Max
	//Height of rise, depth of sinkhole, and depth of crack
	public int heightMin = 1; //Height min
	public int heightMax = 5; //Height max
	//Length of the crack or rise
	public float lengthMin = 1; // Length Min
	public float lengthMax = 5; // Length Max
	public float durationMin = 1.0f; // Duration Min
	public float durationMax = 10.0f;// Duration Max
	public float areaOfEffect = 100.0f;
	public float cutoffDistance = 10.0f;

	//Height list
	private List<int> heightList;
	private int curHeightIndex = 0;
	//Length list
	private List<float> lengthList;
	private int curLengthIndex = 0;
	//Duration list
	private List<float> durationList;
	private int curDurationIndex = 0;
	//Delay list
	private List<float> delayList;
	private int delayIndex=0;
	//Width list
	private List<int> widthList;
	private int widthIndex =0;

	private const int NUM_OF_RAND = 10;


	// Use this for initialization
	void Start () 
	{
		heightList = new List<int>();
		lengthList = new List<float>();
		widthList = new List<int>();
		delayList = new List<float>();
		durationList = new List<float>();

		for(int i = 0; i < NUM_OF_RAND; i++)
		{
			heightList.Add(Random.Range(heightMin,heightMax));
			lengthList.Add(Random.Range(lengthMin,lengthMax));
			widthList.Add(Random.Range(widthMin,widthMax));
			delayList.Add(Random.Range(delayMin,delayMax));
			durationList.Add(Random.Range(durationMin,durationMax));
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public int GetTypeAsInt()
	{
		switch(type)
		{
		case EarthquakeManager.TYPES.RISE:
			return 1;
		case EarthquakeManager.TYPES.SINKHOLE:
			return 0;
		case EarthquakeManager.TYPES.TREMOR:
			return 2;
		}

		return 0;
	}

	public float GetDelay()
	{
		delayIndex++;
		if(delayIndex >= NUM_OF_RAND)
		{
			delayIndex = 0;
		}
		return delayList[delayIndex];
	}

	public float GetDuration()
	{
		curDurationIndex++;
		if(curDurationIndex >= NUM_OF_RAND)
		{
			curDurationIndex=0;
		}
		return durationList[curDurationIndex];
	}

	public int GetHeight()
	{
		curHeightIndex++;
		if(curHeightIndex >= NUM_OF_RAND)
		{
			curHeightIndex =0;
		}
		return heightList[curHeightIndex];
	}

	public float GetLength()
	{
		curLengthIndex++;
		if(curLengthIndex >= NUM_OF_RAND)
		{
			curLengthIndex = 0;
		}
		return lengthList[curLengthIndex];
	}

	public int GetWidth()
	{
		widthIndex++;
		if(widthIndex >= NUM_OF_RAND)
		{
			widthIndex = 0;
		}
		return widthList[widthIndex];
	}

	void OnDrawGizmos()
	{
		Gizmos.color = colour;
		Gizmos.DrawSphere(transform.position,size);
		Gizmos.DrawWireSphere(transform.position,areaOfEffect);
	}
}
