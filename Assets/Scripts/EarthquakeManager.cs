using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EarthquakeManager : MonoBehaviour {
	
	//Earthquake effect script
	//This will simulate an earthquake
	//The designer is able to build the kind of earthquake they want with
	//These properties:
	//	Duration
	//	width- Min & Max
	//	depth- Min & Max
	//	length- Min & Max
	//	cracks- Min & Max
	//	Force
	
	//Lets make this a singleton
	private static EarthquakeManager instance = null;
	public static EarthquakeManager Instance {get{return instance;} }

	public enum TYPES {SINKHOLE,RISE,TREMOR}; 

	private TYPES curType;
//	private vp_Earthquake Earthquake;
	private vp_FPPlayerEventHandler PlayerEvent;
	private vp_FPCamera playerCamera;
	private float startingEarthquakeFactor;
	
	public Terrain terrain;
	
	public Transform player;
	
	public Transform faultZoneContainer;
	
	public AudioClip earthquakeSound;


	public int crackMin = 1; // Crack min
	public int crackMax = 5; // Crack Max
	public int turnsMin = 1; // Turns min
	public int turnsMax = 3; // Turns max

	public int terrainDeformationTextureNum = 1;
	
	private float[,] heightMapBackup;
	private float[, ,] alphaMapBackup;
	private int hmHeight;
	private int hmWidth;
	private float terrHeight;
	private float terrWidth;
	private float terrLength;
	private int alphaMapHeight;
	private int alphaMapWidth;
	protected int numOfAlphaLayers;
	protected const float DEPTH_METER_CONVERT=0.05f;
	protected const float TEXTURE_SIZE_MULTIPLIER = 1.25f;

	//Timer for the quake
	private float curTime;
	private bool isShaking = false;
	private float earthquakeDelayHalf;
	private bool isBuilt = false;
	
	//Direction of the cracks
	private Vector3[,] crackWaypoints;
	//Center point of it
	private Vector3 epicenter;
	private float curAOE;
	private float mapHeight;
	private int curNumCracks;
	private int curHeight;
	private float startHeight;
	private int curWidth;
	private float curEarthquakeDelay;
	private float curDuration;
	private int curTurns; 
	private float curCutOff;
	private const int MAX_MAGNITUDE = 5;
	private const int MIN_MAGNITUDE = 1;
	private float curMagnitude;
	private float curModifier;
	private float playerModifier;

	//Lists filled with random numbers, an optimization so random does not need to be called
	private List<int> crackNumbers;
	private int curCrackListIndex = 0;
	private List<int> turnNumbers;
	private int curTurnListIndex = 0;
	private List<Vector3> directionList;
	private int curDirectionIndex = 0;
	private List<int> magnitudeList;
	private int curMagnitudeListIndex = 0;
	private List<int> faultLineList;
	private int curFaultlineListIndex = 0;


	//Current Faultzone
	private Faultzone[] faultzones;
	private int curFaultzone;

	//Nav Mesh Obstacle
	private NavMeshObstacle obstacle;
	
	//Conversion numbers
	//width/size and height/size
	private float heightConversion;
	private float widthConversion;

	private bool setupDone = false;

	//Earthquake effect
	Vector3 curPos;
	int curWaypointIndex;
	int curCrackIndex;
	
	//States
	private enum STATES{IDLE,BUILD,BEGIN_SHAKE,SHAKING};
	private STATES curState = STATES.IDLE;
	
	void Awake()
	{
		if(instance != null && instance != this)
		{
			Destroy(this.gameObject);
			return;
		}
		else
		{
			instance = this;
		}
		
		PlayerEvent = (vp_FPPlayerEventHandler)player.GetComponentInChildren(typeof(vp_FPPlayerEventHandler));
		playerCamera = (vp_FPCamera)player.GetComponentInChildren(typeof(vp_FPCamera));
		startingEarthquakeFactor = playerCamera.PositionEarthQuakeFactor;
	//	Earthquake = gameObject.GetComponent<vp_Earthquake>();

		obstacle = gameObject.GetComponent<NavMeshObstacle>();
	}
	
	// Use this for initialization
	void Start () 
	{
		//Sets up the backups and such
		LoadInitialValues();
		GetFaultzones();

		//Set up the lists for the random number stuff
		crackNumbers = new List<int>();
		turnNumbers = new List<int>();
		directionList = new List<Vector3>();
		faultLineList = new List<int>();
		magnitudeList = new List<int>();
		
		for(int i=0; i < 10; i++)
		{
			crackNumbers.Add(Random.Range(crackMin,crackMax));
			turnNumbers.Add(Random.Range(turnsMin,turnsMax));
			directionList.Add(Random.insideUnitSphere);
			faultLineList.Add(Random.Range(0,faultzones.Length));
			magnitudeList.Add(Random.Range(MIN_MAGNITUDE,MAX_MAGNITUDE));
		}

		//Set up the arrays
		//Initialize the array so it is long enough to contain as many cracks as the designer wants
		crackWaypoints = new Vector3[crackMax,turnsMax];
		
		heightConversion = hmHeight / terrLength;
		widthConversion = hmWidth / terrWidth;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		//print(curMagnitude + " " + playerCamera.PositionEarthQuakeFactor);

		//This code is here to deal with the order that this and the faultzone scripts Start functions are called
		if(!setupDone)
		{
			curFaultzone = GetNextFaultZone();;
			curType = faultzones[curFaultzone].type;
			curEarthquakeDelay = faultzones[curFaultzone].GetDelay();
			earthquakeDelayHalf = curEarthquakeDelay * 0.5f;
			curTime = curEarthquakeDelay;
			curState = STATES.IDLE;
			setupDone = true;
		}


		switch(curState)
		{
		case STATES.IDLE:
			curTime -= Time.deltaTime;
			
			if(curTime < earthquakeDelayHalf)
			{
				curState = STATES.BUILD;
			}
			
			break;
		case STATES.BUILD:
			curTime -= Time.deltaTime;
			if(!isBuilt)
			{
				curMagnitude = GetNextMagnitude();
				curModifier = curMagnitude / MAX_MAGNITUDE;
				BuildQuakeDetails();

				//Set the epicenter
				epicenter = faultzones[curFaultzone].transform.position;
				epicenter.y = 0;

				if(curType != TYPES.SINKHOLE)
				{
					BuildDirectionVectors();
				}
				else
				{
					crackWaypoints[0,0] = epicenter;
				}
				//Get starting height
				Vector3 startPos = GetRelativeTerrainPositionFromPos(crackWaypoints[0,0],terrain,hmWidth,hmHeight);
				startHeight = terrain.terrainData.GetHeight(Mathf.FloorToInt(startPos.x),Mathf.FloorToInt(startPos.z)) + curHeight;
				
				isBuilt = true;
			}
			
			if(curTime <=0)
			{
				InitiateEarthquake();
				curState = STATES.BEGIN_SHAKE;
			}
			break;
		case STATES.BEGIN_SHAKE:
			audio.Stop();
			audio.pitch = Time.timeScale;
			audio.PlayOneShot(earthquakeSound);
			curState = STATES.SHAKING;
			curTime = curDuration;
			
			//PlayerEvent.Earthquake.TryStart(new Vector3(0.2f,0.2f,10.0f));
			PlayerEvent.Earthquake.TryStart(new Vector3(0.2f,0.2f,10.0f));
			playerModifier = curModifier;
			playerCamera.PositionEarthQuakeFactor = startingEarthquakeFactor * playerModifier;

			if(curType == TYPES.TREMOR)
			{
				obstacle.radius = curWidth;
				obstacle.carving = true;
			}
			else if(curType == TYPES.RISE)
			{
				obstacle.radius = curWidth;
				obstacle.carving = false;
			}
			else
			{
				obstacle.radius = curAOE;
				obstacle.carving = true;
			}

			//print(curMagnitude + " " + playerCamera.PositionEarthQuakeFactor);

			//Reset the modifier so it starts at 1, it will be altered later so the width and the height is altered based on its distance from the epicenter
			curModifier = 1f;

			break;
		case STATES.SHAKING:
			transform.position = curPos;
			curTime -= Time.deltaTime;

			if(curCrackIndex > curNumCracks)
			{
				isShaking = false;
			}
			if(isShaking)
			{
				ShakeTheWorld();
			}

			
			//End the state when the duration is done
			if(curTime <0)
			{
				curFaultzone = GetNextFaultZone();;
				curType = faultzones[curFaultzone].type;
				curEarthquakeDelay = faultzones[curFaultzone].GetDelay();
				earthquakeDelayHalf = curEarthquakeDelay * 0.5f;
				curTime = curEarthquakeDelay;
				curState = STATES.IDLE;
				isShaking = false;
				isBuilt = false;
				
				PlayerEvent.Earthquake.TryStop();
			}
			break;
		}
	}
	
	
	void LoadInitialValues()
	{
		hmHeight = terrain.terrainData.heightmapHeight;
		hmWidth = terrain.terrainData.heightmapWidth;
		alphaMapHeight = terrain.terrainData.alphamapHeight;
		alphaMapWidth = terrain.terrainData.alphamapWidth;

		terrLength = terrain.terrainData.size.z;
		terrHeight = terrain.terrainData.size.y;
		terrWidth = terrain.terrainData.size.x;

		numOfAlphaLayers = terrain.terrainData.alphamapLayers;

		if(Debug.isDebugBuild)
		{
			heightMapBackup = terrain.terrainData.GetHeights(0,0,hmWidth,hmHeight);
			alphaMapBackup = terrain.terrainData.GetAlphamaps(0,0,alphaMapWidth,alphaMapHeight);
		}
	}
	
	void GetFaultzones()
	{
		//NOTE: Unity named this function poorly it also returns the parent’s component.
		Transform[] potentialZones = faultZoneContainer.GetComponentsInChildren<Transform>();
		
		//initialize the waypoints array so that is has enough space to store the nodes.
		faultzones = new Faultzone[ (potentialZones.Length - 1) ];
		
		//loop through the list and copy the nodes into the array.
		//start at 1 instead of 0 to skip the WaypointContainer’s transform.
		for (int i = 1; i < potentialZones.Length; ++i ) 
		{
			faultzones[ i-1 ] = potentialZones[i].GetComponent<Faultzone>();
		}
	}
	
	void InitiateEarthquake()
	{
		curPos = epicenter;
		curCrackIndex = 0;
		curWaypointIndex =0;
		isShaking = true;
	}
	
	void ShakeTheWorld()
	{

		if(curType != TYPES.SINKHOLE)
		{

			Vector3 relativeDistance = crackWaypoints[curCrackIndex,curWaypointIndex] - curPos;
			curPos += (crackWaypoints[curCrackIndex,curWaypointIndex] - curPos).normalized;

			if(curType == TYPES.RISE)
			{
				RaiseTerrain(curPos,Mathf.CeilToInt(curWidth * curModifier));
			}
			else
			{
				DeformTerrain(curPos,curHeight * curModifier,curWidth * curModifier);
				//TextureDeformation(curPos,curWidth * curModifier);
			}
			
			if(relativeDistance.magnitude < 0.5f)
			{
				curWaypointIndex++;
			}

			//Lets reuse the relativeDistance variable
			relativeDistance = epicenter - curPos;
			//If its past the cut off distance then it needs to alter the modifier based on that distance
			if(relativeDistance.magnitude > curCutOff)
			{
				curModifier = curCutOff / relativeDistance.magnitude;
			}

			
			
			if(curWaypointIndex > curTurns)
			{
				curCrackIndex++;
				curWaypointIndex = 0;
				if(curCrackIndex <= curNumCracks)
				{
					curPos = crackWaypoints[curCrackIndex,curWaypointIndex];
				}
			}

			//Lets make sure the player doesnt get forced below the ground
			relativeDistance = player.transform.position - curPos;
			relativeDistance.y = 0;
			//print(relativeDistance.magnitude);
			if(relativeDistance.magnitude < curWidth + 2.0f)
			{
				Vector3 pos = GetRelativeTerrainPositionFromPos(curPos,terrain,hmWidth,hmHeight);
				float height = terrain.terrainData.GetHeight(Mathf.FloorToInt(pos.x),Mathf.FloorToInt(pos.z));

				pos.y = height + 2.0f;
				pos.x = player.transform.position.x;
				pos.z = player.transform.position.z;

				player.transform.position = pos;

			}
		}
		else
		{
			DeformTerrain(epicenter,curHeight,curAOE);
			isShaking = false;
		}
		
		
	}
	
	protected void DeformTerrain(Vector3 pos, float height, float width)
	{
		//get the heights only once keep it and reuse, precalculate as much as possible
		Vector3 terrainPos = GetRelativeTerrainPositionFromPos(pos,terrain,hmWidth,hmHeight);//terr.terrainData.heightmapResolution/terr.terrainData.heightmapWidth
		int heightMapCraterWidth = (int)(width * (widthConversion));
		int heightMapCraterLength = (int)(width * (heightConversion));
		int heightMapStartPosX = (int)(terrainPos.x - (heightMapCraterWidth * 0.5));
		int heightMapStartPosZ = (int)(terrainPos.z - (heightMapCraterLength * 0.5));
		
		float[,] heights = terrain.terrainData.GetHeights(heightMapStartPosX, heightMapStartPosZ, heightMapCraterWidth, heightMapCraterLength);
		float circlePosX;
		float circlePosY;
		float distanceFromCenter;
		float depthMultiplier;

		float widthHalf = width * 0.5f;
		int craterHalfWidth = heightMapCraterWidth / 2;
		int craterHalfLength = heightMapCraterLength / 2;
		
		float deformationDepth = height / terrHeight;
		
		// we set each sample of the terrain in the size to the desired height
		for (int i = 0; i < heightMapCraterLength; i++) //width
		{
			for (int j = 0; j < heightMapCraterWidth; j++) //height
			{
				circlePosX = (j - (craterHalfWidth)) / widthConversion;
				circlePosY = (i - (craterHalfLength)) / heightConversion;
				distanceFromCenter = Mathf.Abs(Mathf.Sqrt(circlePosX * circlePosX + circlePosY * circlePosY));
				//convert back to values without skew
				
				if (distanceFromCenter < widthHalf)
				{
					
					depthMultiplier = ((widthHalf- distanceFromCenter) / widthHalf);
					
					depthMultiplier += 0.1f;
					
					depthMultiplier += Random.value * .1f;
					
					depthMultiplier = Mathf.Clamp(depthMultiplier, 0, 1);
					heights[i, j] = Mathf.Clamp(heights[i, j] - deformationDepth * depthMultiplier, 0, 1);
				}
				
			}
		}
		
		// set the new height
		terrain.terrainData.SetHeights(heightMapStartPosX, heightMapStartPosZ, heights);
	}

	protected void TextureDeformation(Vector3 pos, float craterSizeInMeters)
	{
		Vector3 alphaMapTerrainPos = GetRelativeTerrainPositionFromPos(pos, terrain, alphaMapWidth, alphaMapHeight);
		int alphaMapCraterWidth = (int)(craterSizeInMeters * (alphaMapWidth / terrLength));
		int alphaMapCraterLength = (int)(craterSizeInMeters * (alphaMapHeight / terrLength));
		
		int alphaMapStartPosX = (int)(alphaMapTerrainPos.x - (alphaMapCraterWidth / 2));
		int alphaMapStartPosZ = (int)(alphaMapTerrainPos.z - (alphaMapCraterLength/2));
		
		float[, ,] alphas = terrain.terrainData.GetAlphamaps(alphaMapStartPosX, alphaMapStartPosZ, alphaMapCraterWidth, alphaMapCraterLength);
		
		float circlePosX;
		float circlePosY;
		float distanceFromCenter;
		
		for (int i = 0; i < alphaMapCraterLength; i++) //width
		{
			for (int j = 0; j < alphaMapCraterWidth; j++) //height
			{
				circlePosX = (j - (alphaMapCraterWidth / 2)) / (alphaMapWidth / terrWidth);
				circlePosY = (i - (alphaMapCraterLength / 2)) / (alphaMapHeight / terrLength);
				
				//convert back to values without skew
				distanceFromCenter = Mathf.Abs(Mathf.Sqrt(circlePosX * circlePosX + circlePosY * circlePosY));
				
				
				if (distanceFromCenter < (craterSizeInMeters / 2.0f))
				{
					for (int layerCount = 0; layerCount < numOfAlphaLayers; layerCount++)
					{
						//could add blending here in the future
						if (layerCount == terrainDeformationTextureNum)
						{
							alphas[i, j, layerCount] = 1;
						}
						else
						{
							alphas[i, j, layerCount] = 0;
						}
					}
				}
			}
		} 
		
		terrain.terrainData.SetAlphamaps(alphaMapStartPosX, alphaMapStartPosZ, alphas);
	}
		
	protected void RaiseTerrain(Vector3 pos, int width)
	{
		//get the heights only once keep it and reuse, precalculate as much as possible
		Vector3 terrainPos = GetRelativeTerrainPositionFromPos(pos,terrain,hmWidth,hmHeight);//terr.terrainData.heightmapResolution/terr.terrainData.heightmapWidth
		int heightMapCraterWidth = (int)(width * (widthConversion));
		int heightMapCraterLength = (int)(width * (heightConversion));
		int heightMapStartPosX = (int)(terrainPos.x - (heightMapCraterWidth / 2));
		int heightMapStartPosZ = (int)(terrainPos.z - (heightMapCraterLength / 2));
		
		float[,] heights = terrain.terrainData.GetHeights(heightMapStartPosX, heightMapStartPosZ, heightMapCraterWidth, heightMapCraterLength);
		
		float deformationDepth = ((startHeight) / terrHeight);
		// we set each sample of the terrain in the size to the desired height
		for (int i = 0; i < heightMapCraterLength; i++) //width
		{
			for (int j = 0; j < heightMapCraterWidth; j++) //height
			{
				heights[i,j] = deformationDepth;
			}
		}
		// set the new height
		terrain.terrainData.SetHeights(heightMapStartPosX, heightMapStartPosZ, heights);
	}



	
	void BuildDirectionVectors()
	{
		//Build the direction of the vectors using these values
		Vector3 randomDirection = Vector3.zero;
		
		//Get the maximum values
		float xMax = epicenter.x + (curAOE * 0.5f);
		float xMin = epicenter.x - (curAOE * 0.5f);
		
		float zMax = epicenter.z + (curAOE * 0.5f);
		float zMin = epicenter.z - (curAOE * 0.5f);
		
		//Keep the max and mins within the terrain
		xMax = xMax > terrWidth ? terrWidth : xMax;
		xMin = xMin < 0 ? 0 : xMin;
		
		zMax = zMax > terrLength ? terrLength : zMax;
		zMin = zMin < 0 ? 0 : zMin;
		
		
		curNumCracks = GetNextNumCracks();
		
		//Crack direction is the unit vector of the cracks direction
		//Crack waypoints are the starting and ending points of each crack
		
		
		//Find the begining point of the crack and go
		float length;
		Vector3 startPoint;
		for(int i=0; i < curNumCracks; i++)
		{
			//Create the first point
			//Find the start point for this crack
			startPoint = new Vector3(Random.Range(xMin,xMax),0,Random.Range(zMin,zMax));
			
			crackWaypoints[i,0] = startPoint;
			for(int j = 1; j <= curTurns; j++)
			{
				length = faultzones[curFaultzone].GetLength();
				randomDirection = GetNextDirection();
				//Now build the next way point
				crackWaypoints[i,j] = randomDirection * length;
				crackWaypoints[i,j] += crackWaypoints[i,j-1];
				crackWaypoints[i,j].y = 0;
			}
		}
		
		
	}
	
	
	//This is called by the current faultzone
	public void BuildQuakeDetails()
	{
		curWidth = Mathf.CeilToInt(faultzones[curFaultzone].GetWidth() * curModifier);
		curHeight = Mathf.CeilToInt(faultzones[curFaultzone].GetHeight() * curModifier);
		curDuration = faultzones[curFaultzone].GetDuration() * curModifier;
		curCutOff = faultzones[curFaultzone].cutoffDistance * curModifier;
		curAOE = faultzones[curFaultzone].areaOfEffect * curModifier;
		
		curNumCracks = GetNextNumCracks();
		curTurns = GetNextNumTurns();
	}
	
	
	void OnApplicationQuit()
	{
		if(Debug.isDebugBuild)
		{
			terrain.terrainData.SetHeights(0,0,heightMapBackup);
			terrain.terrainData.SetAlphamaps(0,0,alphaMapBackup);
		}
	}
	
	
	public bool IsShaking()
	{
		return isShaking;
	}
	
	protected Vector3 GetNormalizedPositionRelativeToTerrain(Vector3 pos, Terrain terr)
	{
		//code based on: http://answers.unity3d.com/questions/3633/modifying-terrain-height-under-a-gameobject-at-runtime
		// get the normalized position of this game object relative to the terrain
		Vector3 tempCoord = (pos - terr.gameObject.transform.position);
		Vector3 coord;
		coord.x = tempCoord.x / terrWidth;
		coord.y = tempCoord.y / terrHeight;
		coord.z = tempCoord.z / terrLength;
		
		return coord;
	}
	
	protected Vector3 GetRelativeTerrainPositionFromPos(Vector3 pos,Terrain terr, int mapWidth, int mapHeight)
	{
		Vector3 coord = GetNormalizedPositionRelativeToTerrain(pos, terr);
		// get the position of the terrain heightmap where this game object is
		return new Vector3((coord.x * mapWidth), 0, (coord.z * mapHeight));
	}     


	private Vector3 GetNextDirection()
	{
		curDirectionIndex++;
		if(curDirectionIndex >= directionList.Count)
		{
			curDirectionIndex = Random.Range(0,directionList.Count);
		}

		return directionList[curDirectionIndex];
	}

	private int GetNextNumCracks()
	{
		curCrackListIndex++;
		if(curCrackListIndex >= crackNumbers.Count)
		{
			curCrackListIndex = 0;
		}
		
		return crackNumbers[curCrackListIndex];
	}

	private int GetNextNumTurns()
	{
		curTurnListIndex++;
		if(curTurnListIndex >= turnNumbers.Count)
		{
			curTurnListIndex = 0;
		}
		
		return turnNumbers[curTurnListIndex];
	}

	private int GetNextFaultZone()
	{
		curFaultlineListIndex++;
		if(curFaultlineListIndex >= faultLineList.Count)
		{
			curFaultlineListIndex = Random.Range(0,faultLineList.Count);
		}
		
		return faultLineList[curFaultlineListIndex];
	}

	private int GetNextMagnitude()
	{
		curMagnitudeListIndex++;
		if(curMagnitudeListIndex >= magnitudeList.Count)
		{
			curMagnitudeListIndex = 0;
		}
		
		return magnitudeList[curMagnitudeListIndex];
	}


}

