using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//************************************************************************************************************************************************
//**EZ Parallax script version 1.2 by TimeFloat. Thanks for purchasing! Feel free to scroll to the bottom for user facing funtions.             **
//************************************************************************************************************************************************

[System.Serializable]
public class EZParallaxObjectElement
{
	public string name;
	[SerializeField,]
	private Transform m_parallaxObject;
	public Transform parallaxObject
	{
		set
		{ 	
			m_parallaxObject = value;
			needsNewScreenspaceExtents = true;
		}
		
		get
		{
			return m_parallaxObject;	
		}
	}
	public float privateParallaxSpeedScalarX = 1.0f;
	public float privateParallaxSpeedScalarY = 1.0f;
	public bool isMotorized = false;
	public bool motorizationPaused = false;
	public float motorSpeed { get { return initialMotorSpeed * scale; } }
	
	public bool spawnsDuplicateOnX = false;
	public bool randomSpawnX = false;
	public int spawnGroupIndex = 0;
	public float spawnDistanceX { get { return initialSpawnDistanceX * scale; } }
	public float spawnDistanceMinX { get { return initialSpawnDistanceMinX * scale; } }
	public float spawnDistanceMaxX { get { return initialSpawnDistanceMaxX * scale; } }
	public float rightSpawnDistanceX { get { return initialRightSpawnDistanceX * scale; } }
	public float leftSpawnDistanceX { get { return initialLeftSpawnDistanceX * scale; } }
	public int numElementsInSeriesX = 0;
	
	public float meshExtentXRight { get { return initialMeshExtentXRight * scale; } }
	public float meshExtentXLeft { get { return initialMeshExtentXLeft * scale; } }
	public float meshWidth { get { return meshExtentXRight + meshExtentXLeft; } }

	public float meshExtentY { get { return initialMeshExtentY * scale; } }
	public float meshHeight { get { return meshExtentY * 2.0f; } }
	
	public bool hasCustomName;
	public bool needsNewScreenspaceExtents = false;
	
	private Vector3 initialScale;
	public float initialMotorSpeed = 0;
	public float initialSpawnDistanceX = 0;
	public float initialSpawnDistanceMinX = 0;
	public float initialSpawnDistanceMaxX = 0;
	public float initialRightSpawnDistanceX = 0;
	public float initialLeftSpawnDistanceX = 0;
	private float initialMeshExtentXRight;
	private float initialMeshExtentXLeft;
	private float initialMeshExtentY;
	
	public float altSpawnAnchorY = 0.0f;
	public bool  useRandomYSpawnAltOffset = false;
	public float randomYSpawnLowestAltOffset = 0.0f;
	public float randomYSpawnHighestAltOffset = 0.0f;
	
	public bool isDupe = false;
	
	public float scale = 1.0f;

	public Bounds fullBounds;
	
	public DupeChainHandle dupeChainObject = null;

	public EZParallaxObjectElement(Transform targetTransform)
	{
		if(targetTransform)
		{
			name = targetTransform.name;
			parallaxObject = targetTransform;
			
			initialSpawnDistanceX = Mathf.Abs(initialSpawnDistanceX);
		}
	}
	
	public void Initialize()
	{
		scale = 1.0f;
		
		if(parallaxObject)
		{
			initialScale = parallaxObject.localScale;
			UpdateBounds();
		}
	}

	public Bounds UpdateBounds()
	{
		Renderer targetRenderer = m_parallaxObject.GetComponent<Renderer>();

		Bounds targetBounds = new Bounds(m_parallaxObject.position, Vector3.zero);
		if(targetRenderer != null)
		{
			targetBounds = targetRenderer.bounds;
		}
		
		Renderer[] childRenderers = m_parallaxObject.GetComponentsInChildren<Renderer>();
		if(childRenderers.Length > 0)
		{
			for(int i = 0; i < childRenderers.Length; i++)
			{
				targetBounds.Encapsulate(childRenderers[i].bounds);
			}
		}

		initialMeshExtentXRight = (targetBounds.center.x - m_parallaxObject.position.x) + targetBounds.extents.x;
		initialMeshExtentXLeft = targetBounds.extents.x + (m_parallaxObject.position.x - targetBounds.center.x);
		initialMeshExtentY = targetBounds.extents.y;
		fullBounds = targetBounds;
		return fullBounds;
	}
	
	public void UpdateScale(float newScaleRatio, GameObject mainCam)
	{
		float scalar;
		Transform targetObject;
		Transform tessst;
		Vector3 baseScale;
		
		if(dupeChainObject != null && dupeChainObject.scaleUpdated == false)
		{
			scalar = newScaleRatio / dupeChainObject.macroScale;
			targetObject = dupeChainObject.targetGameObject.transform;
			baseScale = new Vector3(1, 1, 1);
			dupeChainObject.scaleUpdated = true;
			dupeChainObject.macroScale = newScaleRatio;
			scale = newScaleRatio;  //ADDED 9_13
		}
		else if(dupeChainObject != null)
		{
			scale = newScaleRatio;  //ADDED 9_13
			return;
		}
		else
		{
			scalar = newScaleRatio / scale;
			targetObject = parallaxObject;
			baseScale = initialScale;
			scale = newScaleRatio;
		}
		
		targetObject.localScale = new Vector3(baseScale.x * newScaleRatio, baseScale.y * newScaleRatio, baseScale.z);
		
		Vector3 offset = targetObject.position - mainCam.transform.position;
		float z = offset.z;
		offset.z = 0.0f;
		targetObject.position = mainCam.transform.position + offset * scalar + new Vector3(0.0f, 0.0f, z);
	}
	
	public void Update(float dt)
	{
		if(!motorizationPaused && isMotorized && m_parallaxObject)
		{
			Vector3 shiftVector = new Vector3(motorSpeed * dt, 0, 0);
			if(dupeChainObject != null && !dupeChainObject.motorIsPaused && !dupeChainObject.hasMotorShifted)
			{
				dupeChainObject.targetGameObject.transform.position += shiftVector;
				dupeChainObject.hasMotorShifted = true;
			}
			else if(dupeChainObject == null)
			{
				m_parallaxObject.position += shiftVector;
			}
		}
	}
}

public class DupeChainHandle
{
	public bool hasBeenMoved           = false;
	public bool hasMotorShifted        = false;
	public bool motorIsPaused          = false;
	public bool scaleUpdated           = false;
	public float macroScale            = 1;
	public GameObject targetGameObject = null;
	List<EZParallaxObjectElement> parallaxElementsX;

	public EZParallaxObjectElement leftmostObject
	{
		get { return parallaxElementsX[0];}
	}

	public EZParallaxObjectElement rightmostObject
	{
		get { return parallaxElementsX[parallaxElementsX.Count - 1];}
	}

	public enum Axis
	{
		X,
		Y
	}

	public List<EZParallaxObjectElement> GetElementList(EZParallaxObjectElement targetelement)
	{
		return parallaxElementsX;
	}

	public void AddNewParallaxElemToEndOfList(Axis axis, EZParallaxObjectElement newElem)
	{
		if(axis == Axis.X)
		{
			if(parallaxElementsX == null)
			{
				parallaxElementsX = new List<EZParallaxObjectElement>();
			}
			parallaxElementsX.Add(newElem);
		}
	}

	public void AddNewParallaxElemToStartOfList(Axis axis,EZParallaxObjectElement newElem)
	{
		if(axis == Axis.X)
		{
			if(parallaxElementsX == null)
			{
				parallaxElementsX = new List<EZParallaxObjectElement>();
				parallaxElementsX.Add(newElem);
			}
			else
			{
				parallaxElementsX.Insert(0, newElem);
			}
		}
	}

	public void ShiftParallaxItemsRight()
	{
		//For Direction.RIGHT or Direction.CAM_LEFT, usually
		if(parallaxElementsX == null)
		{
			Debug.LogError("EZ Parallax: ERROR -- DupeChainHandle was asked to shift items but has none!");
			return;
		}
		int lastIdx = parallaxElementsX.Count -1;
		EZParallaxObjectElement shiftElem = parallaxElementsX[lastIdx];
		parallaxElementsX.RemoveAt(lastIdx);
		parallaxElementsX.Insert(0, shiftElem);
	}

	public void ShiftParallaxItemsLeft()
	{
		//For Direction.LEFT or Direction.CAM_RIGHT, usually
		if(parallaxElementsX == null)
		{
			Debug.LogError("EZ Parallax: ERROR -- DupeChainHandle was asked to shift items but has none!");
			return;
		}
		EZParallaxObjectElement shiftElem = parallaxElementsX[0];
		parallaxElementsX.RemoveAt(0);
		parallaxElementsX.Add(shiftElem);
	}

	public EZParallaxObjectElement GetEdgeParallaxElement(Axis axis, EZParallax.Direction direction)
	{
		if(direction == EZParallax.Direction.LEFT || direction == EZParallax.Direction.CAM_RIGHT)
		{
			return leftmostObject;
		}
		else
		{
			return rightmostObject;
		}
	}

	public int GetIndexOf(EZParallaxObjectElement targetElem)
	{
		int idx = -1;
		idx = parallaxElementsX.IndexOf(targetElem);
		return idx;
	}

	public EZParallaxObjectElement GetNextElemInChain(Axis axis, EZParallaxObjectElement targetElem, EZParallax.Direction direction, bool wrap)
	{
		EZParallaxObjectElement nextElement = null;
		if(direction == EZParallax.Direction.LEFT)
		{
			int currentIdx = parallaxElementsX.IndexOf(targetElem);
			if(currentIdx == -1)
			{
				Debug.LogError("EZ Parallax: Call to GetNextElemInChain could not find the object named " + targetElem.name + "in it's dupe chain array! Returning null.");
			}
			if(currentIdx > 0)
			{
				nextElement = parallaxElementsX[currentIdx - 1];
			}
			else if(wrap)
			{
				nextElement = parallaxElementsX[parallaxElementsX.Count - 1];
			}
		}
		else if(direction == EZParallax.Direction.RIGHT)
		{
			int currentIdx = parallaxElementsX.IndexOf(targetElem);
			if(currentIdx < parallaxElementsX.Count - 1)
			{
				nextElement = parallaxElementsX[currentIdx + 1];
			}
			else if(wrap)
			{
				nextElement = parallaxElementsX[0];
			}
		}

		return nextElement;
	}
}

public class EZParallax : MonoBehaviour
{
	public string                    m_parallaxingTagName;
	public string                    m_wrapXParallaxingTagName;
	public GameObject                m_mainCamera;
	private Camera                   m_mainCameraCam;
	private bool                     m_camTypeIsOrtho;
	public GameObject                m_playerObj;
	public float                     m_parallaxSpeedScalarX = 1;
	public float                     m_parallaxSpeedScalarY = 1;
	public bool                      m_autoInitialize = true;
	public bool                      m_enableDollyZoom = true;
	public EZParallaxObjectElement[] m_parallaxElements;
	private DupeChainHandle[]        m_dupeChainHandles; 
	
	private float                    m_maxDist;
	private float                    m_maxDistDiv;
	private Vector3                  m_camStartVect;
	private float                    m_camStartOrthoSize;
	private float                    m_prevOrthoSize;
	private float                    m_currentOrthoSize;
	private bool                     m_initialized = false;
	
	private Vector3                  m_currentCameraPosition;
	private Vector3                  m_previousCameraPosition;
	
	private int[]                    m_rndDistArrayIndex;
	private int[]                    m_rndAltDistArrayIndex;
	private int[]                    m_rndElementGroupSize;
	private int                      m_rndDistStartIndex;
	public int                       m_randomOffsetHistorySize = 300;
	private float?[,]                m_rndDistArray;
	private float?[,]                m_rndAltDistArray;
	private int                      m_randomSpawnCtr = 0;

	private float                    m_screenRatioBufferTotal = 3.0f;

	private float 					 m_unitToPixelRatioBase;

	public enum Direction
	{
		LEFT,
		RIGHT,
		UP,
		DOWN,
		CAM_LEFT,
		CAM_RIGHT,
		CAM_UP,
		CAM_DOWN,
		COUNT
	}
	
	private float GetUnitToPixelRatioAtZ(float z)
	{
		if(m_camTypeIsOrtho)
		{
			return m_unitToPixelRatioBase;
		}
		else
		{
			return m_unitToPixelRatioBase * z;
		}
	}

	private float UpdateUnitToPixelRatioBase()
	{
		Vector3 pointA = m_mainCameraCam.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, 1.0f));
		Vector3 pointB = m_mainCameraCam.ScreenToWorldPoint(new Vector3(1.0f, 0.0f, 1.0f));
		m_unitToPixelRatioBase = (1.0f / Mathf.Abs(pointA.x - pointB.x));
		return m_unitToPixelRatioBase;
	}
	
	void Start()
	{
		if(m_autoInitialize){
			InitializeParallax();
		}
	}
	
	//Can be called manually when autoInitialization is disabled.
	public void InitializeParallax()
	{
		if(m_playerObj == null)
		{
			Debug.LogError("EZ Parallax: EZParallax initialized, but a player has not been assigned. Aborting.");
			return;
		}
		
		if(m_mainCamera == null)
		{
			Debug.LogError("EZ Parallax: EZParallax initialized, but a camera has not been assigned. Aborting.");
			return;
		}
		m_mainCameraCam = m_mainCamera.GetComponent<Camera>();
		m_camTypeIsOrtho = m_mainCameraCam.orthographic;
		PurgeDupes();
		m_randomSpawnCtr = 0;
		if(!m_initialized)
		{
			AddTaggedElements();
		}
		
		if(m_parallaxElements.Length == 0)
		{
			Debug.LogWarning("EZ Parallax: EZParallax initialized, but no objects have been assigned! No parallaxing effects will be present.");
			return;
		}
		
		for(int i = 0; i < m_parallaxElements.Length; i++)
		{
			if(m_parallaxElements[i].parallaxObject)
			{
				if ( (m_parallaxElements[i].parallaxObject.position.z - m_playerObj.transform.position.z) == 0.0f )
				{
					Debug.LogError("EZ Parallax: EZParallax initialized, but a parallaxing object, " + m_parallaxElements[i].name + ", has the same Z axis position as the player. Move this object to a different z depth to allow EZ Parallax to work properly. Aborting.");
					return;
				}
			}
		}
		
		SqueezeElementsArray();
		int randomSpawningCount = 0;
		for(int i = 0; i < m_parallaxElements.Length; i++)
		{
			m_parallaxElements[i].Initialize();
			if(m_parallaxElements[i].randomSpawnX)
			{
				randomSpawningCount++;
			}
		}
		m_rndDistStartIndex = Mathf.CeilToInt(m_randomOffsetHistorySize / 2);
		m_rndDistArrayIndex = new int[randomSpawningCount];
		m_rndAltDistArrayIndex = new int[randomSpawningCount];
		m_rndElementGroupSize = new int[randomSpawningCount];
		m_rndDistArray = new float?[randomSpawningCount, m_randomOffsetHistorySize];
		m_rndAltDistArray = new float?[randomSpawningCount, m_randomOffsetHistorySize];
		EstablishMaxDistance();
		m_camStartVect = m_mainCamera.transform.position;
		m_previousCameraPosition = m_camStartVect;
		m_camStartOrthoSize = m_mainCamera.GetComponent<Camera>().orthographicSize;
		if(m_camStartOrthoSize <= 0.0f)
		{
			Debug.LogError("Camera assigned to EZ Parallax has a size of 0 or less. Aborting initialization. Reinitialize with proper camera settings for parallax effect.");
			return;
		}
		m_currentOrthoSize = m_camStartOrthoSize;
		m_prevOrthoSize = m_camStartOrthoSize;
		UpdateUnitToPixelRatioBase();
		SpawnDupes();
		for(int i = 0; i < m_parallaxElements.Length; i++)
		{
			if(m_parallaxElements[i].spawnsDuplicateOnX)
			{
				UpdateDupeObjects(m_parallaxElements[i], Direction.RIGHT);
			}
		}
		m_initialized = true;
	}
	
	void SpawnDupes()
	{
		float screenWidth = m_mainCameraCam.pixelWidth;
		List<EZParallaxObjectElement> elementsToDupe = new List<EZParallaxObjectElement>();
		
		for(int i = 0; i < m_parallaxElements.Length; i++)
		{
			if(m_parallaxElements[i].spawnsDuplicateOnX)
			{
				elementsToDupe.Add(m_parallaxElements[i]);
			}
		}
		
		
		for(int i = 0; i < elementsToDupe.Count; i++)
		{
			SpawnSingleElementDupes(elementsToDupe[i], m_mainCameraCam, screenWidth);
		}
	}
	
	void SpawnSingleElementDupes(EZParallaxObjectElement targetElement, Camera m_mainCameraCam, float screenWidth)
	{
		DupeChainHandle dupeChainParent;
		
		if(targetElement.dupeChainObject == null)
		{
			dupeChainParent = new DupeChainHandle();
			dupeChainParent.targetGameObject = new GameObject();
			dupeChainParent.targetGameObject.name = "EZP " + targetElement.name + " Dupe Handle";
			Transform origParent = targetElement.parallaxObject.parent;
			targetElement.parallaxObject.parent = dupeChainParent.targetGameObject.transform;
			dupeChainParent.targetGameObject.transform.parent = origParent;
			targetElement.dupeChainObject = dupeChainParent;
			
			if(m_dupeChainHandles != null)
			{
				DupeChainHandle[] tempDCArray = new DupeChainHandle[m_dupeChainHandles.Length + 1];
				for(int i = 0; i < m_dupeChainHandles.Length; i++)
				{
					tempDCArray[i] = m_dupeChainHandles[i];
				}
				
				tempDCArray[m_dupeChainHandles.Length] = dupeChainParent;
				m_dupeChainHandles = tempDCArray;
			}
			else
			{
				m_dupeChainHandles = new DupeChainHandle[1];
				m_dupeChainHandles[0] = dupeChainParent;
			}
			dupeChainParent.AddNewParallaxElemToStartOfList(DupeChainHandle.Axis.X, targetElement);
		}
		else
		{
			dupeChainParent = targetElement.dupeChainObject;
		}
		
		targetElement.altSpawnAnchorY = targetElement.parallaxObject.position.y;

		float targetElementZ = targetElement.parallaxObject.position.z;
		float targetObjectUnitToPixelRatio = GetUnitToPixelRatioAtZ(targetElementZ);
		float targetElementScreenWidth = targetElement.meshWidth * targetObjectUnitToPixelRatio;
		Vector2 targetCenterPosScreenPt = m_mainCameraCam.WorldToScreenPoint(targetElement.parallaxObject.position);

		//Create an array of randomized offsets for randomized placement use. If spawning is random, use this array to figure out how wide the initial positioning needs to be, then draw from that array when spawning the individual elements.
		List<float> randomOffsetsList = new List<float>();
		
		int maxDupes = 0;
		
		int numRightDupes = 0;
		int numLeftDupes = 0;
		
		if (targetElement.randomSpawnX)
		{
			//Goof check
			if(targetElement.initialSpawnDistanceMinX > targetElement.initialSpawnDistanceMaxX)
			{
				Debug.LogWarning ("EZParallax: WARNING -- For your " + targetElement.name + " element, your minimum random spawn distance is greater than your maximum random spawn distance. Automatically swapping your minimum for your maximum.");
				float swapVar = targetElement.initialSpawnDistanceMinX;
				targetElement.initialSpawnDistanceMinX = targetElement.initialSpawnDistanceMaxX;
				targetElement.initialSpawnDistanceMaxX = swapVar;
			}

			float minScreenSpcOffset = targetElement.spawnDistanceMinX * targetObjectUnitToPixelRatio;
			maxDupes = Mathf.CeilToInt( ( screenWidth * m_screenRatioBufferTotal )/ (targetElementScreenWidth + minScreenSpcOffset)) + 2; // * 3 screenwidth (m_screenRatioBufferTotal) and +2 for buffer objects
			
			for(int k = 0; k < m_randomOffsetHistorySize; k++)
			{
				float randomOffset = Random.Range(targetElement.spawnDistanceMinX, targetElement.spawnDistanceMaxX);
				randomOffsetsList.Add(randomOffset);
				m_rndDistArray[m_randomSpawnCtr, k] = randomOffset;
				
				//For optional use with alternate offsets, currently only available for randomized Y offsets
				float randomAltOffset = Random.Range(targetElement.randomYSpawnLowestAltOffset, targetElement.randomYSpawnHighestAltOffset);
				m_rndAltDistArray[m_randomSpawnCtr, k] = randomAltOffset;
			}

			m_rndAltDistArray[m_randomSpawnCtr, m_rndDistStartIndex] = 0.0f; //Don't want an offset from the first, alt position anchoring, object.
			m_rndElementGroupSize[m_randomSpawnCtr] = maxDupes; //Does not include the original object that was spawned from
			GetNumLeftAndRightDupes(targetCenterPosScreenPt, targetElementScreenWidth, screenWidth, maxDupes, out numLeftDupes, out numRightDupes);
			m_rndDistArrayIndex[m_randomSpawnCtr] = m_rndDistStartIndex + numRightDupes - 1; //Subtract 1 to make room for the initial index shift when the character starts moving, pos or neg
			m_rndAltDistArrayIndex[m_randomSpawnCtr] = m_rndDistArrayIndex[m_randomSpawnCtr] + 1;
			targetElement.spawnGroupIndex = m_randomSpawnCtr;
			m_randomSpawnCtr++;
			
			if(m_rndElementGroupSize[m_randomSpawnCtr - 1] >= m_randomOffsetHistorySize)
			{
				Debug.LogError ("EZParallax: An EZParallax element object named " + targetElement.name + " needs to spawn more objects than there are slots in the random offset history! Raise your history size to greater than " + maxDupes + " to resolve this problem. Aborting the creation of duplicate objects for " + targetElement.name + ".");
				return;
			}
		}
		else
		{
			maxDupes = Mathf.CeilToInt( ( screenWidth * m_screenRatioBufferTotal ) / ( targetElementScreenWidth + targetElement.spawnDistanceX * targetObjectUnitToPixelRatio ) ) + 2; // +2 buffer objects
			targetElement.spawnGroupIndex = 0;
			GetNumLeftAndRightDupes(targetCenterPosScreenPt, targetElementScreenWidth, screenWidth, maxDupes, out numLeftDupes, out numRightDupes);
		}

		//Spawn all objects on the left side
		SpawnAllObjectsOnOneSide(randomOffsetsList, dupeChainParent, targetElement, numLeftDupes, Direction.LEFT);
		//Spawn all objects on the right side
		SpawnAllObjectsOnOneSide(randomOffsetsList, dupeChainParent, targetElement, numRightDupes, Direction.RIGHT);
	}

	EZParallaxObjectElement SpawnAllObjectsOnOneSide(List<float> randomOffsetsList, DupeChainHandle dupeChainParent, EZParallaxObjectElement targetElement, int numberOfDupes, Direction direction)
	{
		EZParallaxObjectElement dupeTargetElement = targetElement;
		EZParallaxObjectElement previousElement = targetElement;
		EZParallaxObjectElement dupeTargetNextDirectionalDupe;
		float directionFlip = 1.0f;

		dupeTargetNextDirectionalDupe = dupeTargetElement.dupeChainObject.GetNextElemInChain(DupeChainHandle.Axis.X, dupeTargetElement, direction, false);

		if(direction == Direction.LEFT)
		{
			directionFlip = -1.0f;
		}

		for ( int k = 0; k < numberOfDupes; ++k )
		{
			if( dupeTargetNextDirectionalDupe != null)
			{
				dupeTargetElement = dupeTargetNextDirectionalDupe;
				continue;
			}
			
			int offsetListShiftIdx = m_rndDistStartIndex + k;
			int offsetIndex = m_rndDistStartIndex - 1 - k;
			bool isRandomSpawning = dupeTargetElement.randomSpawnX;
			Vector3 objectOffsetVector;

			if(direction == Direction.RIGHT)
			{
				objectOffsetVector =                                      (isRandomSpawning) ? new Vector3(randomOffsetsList[offsetListShiftIdx] + dupeTargetElement.meshWidth, 0, 0)                      : new Vector3(targetElement.spawnDistanceX + dupeTargetElement.meshWidth, 0, 0);
				dupeTargetElement.initialRightSpawnDistanceX =            (isRandomSpawning) ? randomOffsetsList[offsetListShiftIdx]                                                                       : dupeTargetElement.spawnDistanceX;
			}
			else
			{
				objectOffsetVector =                                      (isRandomSpawning) ? new Vector3(randomOffsetsList[offsetIndex] + dupeTargetElement.meshWidth, 0, 0)                             : new Vector3(targetElement.spawnDistanceX + dupeTargetElement.meshWidth, 0, 0);
				dupeTargetElement.initialLeftSpawnDistanceX =             (isRandomSpawning) ? randomOffsetsList[offsetIndex]                                                                              : dupeTargetElement.spawnDistanceX;
			}
			
			Transform nextDupeObject = (Transform)(Instantiate(dupeTargetElement.parallaxObject, dupeTargetElement.parallaxObject.position + (objectOffsetVector * directionFlip), dupeTargetElement.parallaxObject.rotation) );
			if(dupeTargetElement.useRandomYSpawnAltOffset)
			{	
				int modifiedIdx = (direction == Direction.RIGHT) ? offsetListShiftIdx + 1 : offsetIndex;
				nextDupeObject.position = new Vector3(nextDupeObject.position.x, targetElement.altSpawnAnchorY + (float)m_rndAltDistArray[dupeTargetElement.spawnGroupIndex, modifiedIdx], nextDupeObject.position.z); //+1 to compensate for offset vs actual positions in array, skipping the starting object
			}
			previousElement = InitializeAssignForNextInChainAndReturnPrev(dupeChainParent, targetElement, dupeTargetElement, previousElement, nextDupeObject, direction);
			dupeTargetElement = previousElement;
			if(direction == Direction.RIGHT)
			{
				dupeChainParent.AddNewParallaxElemToEndOfList(DupeChainHandle.Axis.X, dupeTargetElement);
			}
			else
			{
				dupeChainParent.AddNewParallaxElemToStartOfList(DupeChainHandle.Axis.X, dupeTargetElement);
			}
		}
		return dupeTargetElement; //Returns the last in the chain.
	}

	EZParallaxObjectElement InitializeAssignForNextInChainAndReturnPrev(DupeChainHandle dupeChainParent, EZParallaxObjectElement originalTargetElement, EZParallaxObjectElement dupeTargetElement, EZParallaxObjectElement previousElement, Transform nextDupeObject, Direction direction)
	{
		nextDupeObject.transform.parent =  dupeTargetElement.parallaxObject.parent;
		EZParallaxObjectElement newElement = AddNewParallaxingElement(nextDupeObject);
		newElement.Initialize();
		if(direction == Direction.RIGHT)
		{
			newElement.initialLeftSpawnDistanceX = dupeTargetElement.rightSpawnDistanceX;
		}
		else
		{
			newElement.initialRightSpawnDistanceX = dupeTargetElement.leftSpawnDistanceX;
		}
		dupeTargetElement = newElement;
		dupeTargetElement.isMotorized = originalTargetElement.isMotorized;
		dupeTargetElement.initialMotorSpeed = originalTargetElement.initialMotorSpeed;
		dupeTargetElement.randomSpawnX = originalTargetElement.randomSpawnX;
		dupeTargetElement.useRandomYSpawnAltOffset = originalTargetElement.useRandomYSpawnAltOffset;
		dupeTargetElement.randomYSpawnLowestAltOffset = originalTargetElement.randomYSpawnLowestAltOffset;
		dupeTargetElement.randomYSpawnHighestAltOffset = originalTargetElement.randomYSpawnHighestAltOffset;
		dupeTargetElement.altSpawnAnchorY = originalTargetElement.altSpawnAnchorY;
		dupeTargetElement.spawnGroupIndex = originalTargetElement.spawnGroupIndex;
		dupeTargetElement.privateParallaxSpeedScalarX = originalTargetElement.privateParallaxSpeedScalarX;
		dupeTargetElement.privateParallaxSpeedScalarY = originalTargetElement.privateParallaxSpeedScalarY;
		dupeTargetElement.dupeChainObject = dupeChainParent;
		dupeTargetElement.isDupe = true;

		return dupeTargetElement;
	}
	
	void GetNumLeftAndRightDupes(Vector2 targetCenterPosScreenPt, float targetElementScreenWidth, float screenWidth, int maxDupes, out int numLeftDupes, out int numRightDupes)
	{
		float singleSideScreenBufferSpace = m_screenRatioBufferTotal / 2;
		numRightDupes = Mathf.Min( Mathf.CeilToInt( ( (screenWidth * singleSideScreenBufferSpace) - targetCenterPosScreenPt.x) / targetElementScreenWidth), maxDupes);
		if(numRightDupes < 0)
		{
			numRightDupes = 0;
		}
		numLeftDupes = maxDupes - numRightDupes;
		if(numLeftDupes < 0)
		{
			numLeftDupes = 0;
		}
	}
	
	void AddTaggedElements()
	{
		GameObject[] taggedElements = GameObject.FindGameObjectsWithTag(m_parallaxingTagName);
		GameObject[] taggedWrapXElements = null;
		
		int totalElementSum = taggedElements.Length;
		if(m_wrapXParallaxingTagName != "" && m_wrapXParallaxingTagName != null)
		{
			taggedWrapXElements = GameObject.FindGameObjectsWithTag(m_wrapXParallaxingTagName);
			totalElementSum += taggedWrapXElements.Length;
		}		
		
		if(totalElementSum == 0)
		{
			return;	
		}
		
		List<EZParallaxObjectElement> tempElementArray = new List<EZParallaxObjectElement>(m_parallaxElements);
		
		for(int i = 0; i < taggedElements.Length; i++)
		{
			if(!tempElementArray.Exists( ( EZParallaxObjectElement elem ) => { return elem.parallaxObject == taggedElements[i].transform; } ))
			{
				tempElementArray.Add(new EZParallaxObjectElement(taggedElements[i].transform));
			}
		}

		/*  Flash safe:
		for(int i = 0; i < taggedElements.Length; i++)
		{
			bool test = false;
			for(int j = 0; j < tempElementArray.Count; j++)
			{
				if(tempElementArray[j].parallaxObject == taggedElements[i].transform)
				{
					test = true;
					break;
				}
			}
			if(!test)
			{
				tempElementArray.Add(new EZParallaxObjectElement(taggedElements[i].transform));
			}
		}*/
		
		EZParallaxObjectElement newElement;
		
		if(taggedWrapXElements != null)
		{
			for(int i = 0; i < taggedWrapXElements.Length; i++)
			{
				if(!tempElementArray.Exists( ( EZParallaxObjectElement elem ) => { return elem.parallaxObject == taggedWrapXElements[i].transform; } ))
				{
					newElement = new EZParallaxObjectElement(taggedWrapXElements[i].transform);
					tempElementArray.Add(newElement);
					SetElementWrapSettings(newElement, true, 0);
				}
			}
		}

		/*  Flash safe:
		if(taggedWrapXElements != null)
		{
			for(int i = 0; i < taggedWrapXElements.Length; i++)
			{
				bool test = false;
				for(int j = 0; j < tempElementArray.Count; j++)
				{
					if(tempElementArray[j].parallaxObject == taggedWrapXElements[i].transform)
					{
						test = true;
						break;
					}
				}
				if(!test)
				{
					newElement = new EZParallaxObjectElement(taggedWrapXElements[i].transform);
					tempElementArray.Add(newElement);
					SetElementWrapSettings(newElement, true, 0);
				}
			}
		}*/
		
		m_parallaxElements = tempElementArray.ToArray();		
	}
	
	void LateUpdate()
	{
		if ( m_mainCamera == null )
			return;
		
		if ( !m_initialized )
			return;
		
		m_currentCameraPosition = m_mainCamera.transform.position;
		Vector3 camFrameDelta = m_currentCameraPosition - m_previousCameraPosition;
		m_currentOrthoSize = m_mainCameraCam.orthographicSize;
		if(m_currentOrthoSize <= 0.0f)
		{
			Debug.LogError("Camera assigned to EZ Parallax has a size of 0 or less. Aborting update loop and setting object to uninitialized. Reinitialize with proper camera settings for parallax effect.");
			m_initialized = false;
			return;
		}
		if(m_currentOrthoSize != m_prevOrthoSize)
		{
			UpdateUnitToPixelRatioBase();
		}
		if(camFrameDelta !=  Vector3.zero )
		{
			for(int i = 0; i < m_parallaxElements.Length; i++)
			{
				Direction direction = (camFrameDelta.x >= 0) ? Direction.CAM_RIGHT : Direction.CAM_LEFT;
				EZParallaxObjectElement targetPE = m_parallaxElements[i];
				if(targetPE.parallaxObject == null)
				{
					Debug.LogWarning ("EZ Parallax: WARNING -- There is a parallax object, named " + targetPE.name + ", in your the EZ Parallax element list that doesn't have an actual transform attached to it. This is impossible to achieve from the EZP inspector, so your code may be causing this issue at runtime.");
					continue;
				}
				targetPE.Update(Time.deltaTime);
				float movementScalar = GetElementMovementScalar(targetPE);
				
				UpdateAltSpawnAnchors(targetPE, camFrameDelta);
				UpdateElementWorldPosition(targetPE, camFrameDelta, movementScalar);

				if(targetPE.spawnsDuplicateOnX)
				{
					if(targetPE.isMotorized)
					{
						float camVelocityX = camFrameDelta.x * ( 1 / Time.deltaTime);
						float movementWithScalar = camVelocityX - (camVelocityX * movementScalar);
						direction = (targetPE.motorSpeed + movementWithScalar - camVelocityX ) < 0 ? Direction.LEFT : Direction.RIGHT;
					}
					
					UpdateDupeObjects(targetPE,direction);
				}
			}
			
			m_previousCameraPosition = m_currentCameraPosition;
		}
		else
		{
			for (int i = 0; i < m_parallaxElements.Length; i++)
			{
				EZParallaxObjectElement targetPE = m_parallaxElements[i];
				float movementScalar = GetElementMovementScalar(targetPE);
				if(targetPE.parallaxObject)
				{
					targetPE.Update(Time.deltaTime);
					if(targetPE.spawnsDuplicateOnX && targetPE.isMotorized)
					{
						Direction direction = (targetPE.motorSpeed < 0) ? Direction.LEFT : Direction.RIGHT;
						UpdateDupeObjects(targetPE, direction);
					}
					
					UpdateElementWorldPosition(targetPE, camFrameDelta, movementScalar);
				}
			}
		}
		
		if(m_dupeChainHandles != null)
		{
			for(int i = 0; i < m_dupeChainHandles.Length; i++)
			{
				m_dupeChainHandles[i].hasBeenMoved = false;
				m_dupeChainHandles[i].hasMotorShifted = false;
				m_dupeChainHandles[i].scaleUpdated = false;
			}
		}
		m_prevOrthoSize = m_currentOrthoSize;
	}
	
	void UpdateAltSpawnAnchors(EZParallaxObjectElement targetPE, Vector3 camDelta)
	{
		//This function will eventually include anchor point shifting for x, once y wrapping has been added.
		targetPE.altSpawnAnchorY += camDelta.y;
	}
	
	float GetElementMovementScalar(EZParallaxObjectElement targetPE)
	{
		Transform targetElem = targetPE.parallaxObject;
		float elemToPlayerZ = Mathf.Abs(targetElem.position.z - m_playerObj.transform.position.z);
		float modifiedSpeedScalar;
		if(m_playerObj.transform.position.z > targetElem.position.z)
		{
			//If between the player and the camera
			modifiedSpeedScalar = ( (m_maxDist + elemToPlayerZ) / m_maxDist );
		}
		else
		{
			//if between the player and the farthest bg element, including the furthest bg element
			modifiedSpeedScalar = Mathf.Abs( (m_maxDist - elemToPlayerZ) / m_maxDist);
		}
		
		return modifiedSpeedScalar;
	}
	
	void UpdateTargetScale(EZParallaxObjectElement targetElement)
	{
		Transform targetElemTransform = targetElement.parallaxObject;
		float currentOrthoSizeRatio = 1;
		if(m_currentOrthoSize != m_prevOrthoSize)
		{
			currentOrthoSizeRatio = 1 + ( (m_mainCameraCam.orthographicSize / m_camStartOrthoSize) - 1) * (Mathf.Abs(targetElemTransform.position.z - m_playerObj.transform.position.z) * m_maxDistDiv);
			if(m_enableDollyZoom)
			{
				targetElement.UpdateScale(currentOrthoSizeRatio, m_mainCamera);
			}
		}
	}
	
	void UpdateElementWorldPosition(EZParallaxObjectElement targetPE, Vector3 camFrameDelta, float elemMovementScalar)
	{
		
		Transform targetElem = targetPE.parallaxObject;
		if(targetPE.dupeChainObject != null && targetPE.dupeChainObject.hasBeenMoved != true)
		{
			targetElem = targetPE.dupeChainObject.targetGameObject.transform;
			targetPE.dupeChainObject.hasBeenMoved = true;
		}
		else if(targetPE.dupeChainObject != null)
		{
			UpdateTargetScale(targetPE);
			return;
		}
		
		float modifiedSpeedScalar = elemMovementScalar;
		Vector3 newPosition;
		UpdateTargetScale(targetPE);
		
		float newXPos = (camFrameDelta.x - ( camFrameDelta.x * modifiedSpeedScalar)) * m_parallaxSpeedScalarX * targetPE.privateParallaxSpeedScalarX;
		newPosition = new Vector3(newXPos, ( camFrameDelta.y - (camFrameDelta.y * modifiedSpeedScalar)) * m_parallaxSpeedScalarY * targetPE.privateParallaxSpeedScalarY, 0);
		targetElem.position += newPosition;
	}
	
	void UpdateDupeObjects(EZParallaxObjectElement targetPE, Direction direction)
	{
		Direction oppositeDirection = Direction.RIGHT;
		float targetMeshExtent = targetPE.meshExtentXRight;
		if(direction == Direction.RIGHT)
		{
			oppositeDirection = Direction.LEFT;
			targetMeshExtent = targetPE.meshExtentXLeft;
		}
		else if(direction == Direction.LEFT)
		{
			oppositeDirection = Direction.RIGHT;

		}
		else if(direction == Direction.CAM_RIGHT)
		{
			oppositeDirection = Direction.CAM_LEFT;
		}
		else if(direction == Direction.CAM_LEFT)
		{
			oppositeDirection = Direction.CAM_RIGHT;
			targetMeshExtent = targetPE.meshExtentXLeft;
		}

		EZParallaxObjectElement edgeElement = targetPE.dupeChainObject.GetEdgeParallaxElement(DupeChainHandle.Axis.X, direction);
		EZParallaxObjectElement prevEdgeElement = targetPE.dupeChainObject.GetEdgeParallaxElement(DupeChainHandle.Axis.X, oppositeDirection);

		float screenWidth = m_mainCameraCam.pixelWidth;
		float newSpawnOffset = 0.0f;
		float objectScreenPosX = 0.0f;
		float newYPos = 0.0f;
		if(edgeElement.parallaxObject != null)
		{
			objectScreenPosX = m_mainCameraCam.WorldToScreenPoint(prevEdgeElement.parallaxObject.position).x; // CHANGED to PREV 9/13
		}
		else
		{
			Debug.LogWarning("EZ Parallax: WARNING -- UpdateDupeObjects is trying to access a parallax object, for the EZP object element " + edgeElement.name + ", that doesn't exist! Object may have been destroyed during runtime. objectScreenPosX is remaining 0.");
		}
		
		float elementScreenSpaceExtentX = targetMeshExtent * GetUnitToPixelRatioAtZ(targetPE.parallaxObject.position.z);
		float edgeCheckBuffer = elementScreenSpaceExtentX * 0.2f;

		if(direction == Direction.LEFT || direction == Direction.CAM_RIGHT) //Element is moving to the left side of the screen. Camera most likely is moving to the right.
		{
			while(objectScreenPosX + elementScreenSpaceExtentX <= screenWidth + edgeCheckBuffer )
			{
				if(targetPE.randomSpawnX)
				{
					int spawnGroupIdx = targetPE.spawnGroupIndex;
					
					if(m_rndDistArrayIndex[spawnGroupIdx] == m_randomOffsetHistorySize)
					{
						m_rndDistArrayIndex[spawnGroupIdx] = 0;
					}
					else
					{
						m_rndDistArrayIndex[spawnGroupIdx]++;
					}
					
					if(m_rndAltDistArrayIndex[spawnGroupIdx] == m_randomOffsetHistorySize)
					{
						m_rndAltDistArrayIndex[spawnGroupIdx] = 0;
					}
					else
					{
						m_rndAltDistArrayIndex[spawnGroupIdx]++;
					}
					
					if(!m_rndDistArray[spawnGroupIdx, m_rndDistArrayIndex[spawnGroupIdx]].HasValue)
					{
						//Random offset not stored for this index. Make a new one and store it at this index.
						newSpawnOffset = Random.Range(targetPE.spawnDistanceMinX, targetPE.spawnDistanceMaxX);
						m_rndDistArray[spawnGroupIdx, m_rndDistArrayIndex[spawnGroupIdx]] = newSpawnOffset;
						
						
						float rndAltYOffset = Random.Range(targetPE.randomYSpawnLowestAltOffset, targetPE.randomYSpawnHighestAltOffset);
						m_rndAltDistArray[spawnGroupIdx, m_rndAltDistArrayIndex[spawnGroupIdx]] = rndAltYOffset;
						if(targetPE.useRandomYSpawnAltOffset)
						{
							newYPos = targetPE.altSpawnAnchorY + (rndAltYOffset * targetPE.scale);
						}
						
					}
					else
					{
						newSpawnOffset = (float)m_rndDistArray[spawnGroupIdx, m_rndDistArrayIndex[spawnGroupIdx]];
						if(targetPE.useRandomYSpawnAltOffset)
						{
							newYPos = targetPE.altSpawnAnchorY + ((float)m_rndAltDistArray[spawnGroupIdx, m_rndAltDistArrayIndex[spawnGroupIdx]] * targetPE.scale);
						}
					}
				}
				else
				{
					newSpawnOffset = targetPE.spawnDistanceX;
				}
				
				edgeElement.parallaxObject.position = prevEdgeElement.parallaxObject.position + new Vector3(edgeElement.meshWidth + newSpawnOffset, 0.0f, 0.0f);
				if(edgeElement.useRandomYSpawnAltOffset)
				{
					edgeElement.parallaxObject.position = new Vector3( edgeElement.parallaxObject.position.x, newYPos, edgeElement.parallaxObject.position.z);
				}
				edgeElement.initialSpawnDistanceX = newSpawnOffset;
				
				targetPE.dupeChainObject.ShiftParallaxItemsLeft();
				
				edgeElement = targetPE.dupeChainObject.GetEdgeParallaxElement(DupeChainHandle.Axis.X, direction);
				prevEdgeElement = targetPE.dupeChainObject.GetEdgeParallaxElement(DupeChainHandle.Axis.X, oppositeDirection);

				objectScreenPosX = m_mainCameraCam.WorldToScreenPoint(prevEdgeElement.parallaxObject.position).x;
			}
		}
		else //Element is moving to the right side of the screen. Camera most likely is moving to the left.
		{
			while( objectScreenPosX - elementScreenSpaceExtentX >= -edgeCheckBuffer)
			{
				if(targetPE.randomSpawnX)
				{
					int spawnGroupIdx = targetPE.spawnGroupIndex;
					
					int indexLeftShift = m_rndDistArrayIndex[spawnGroupIdx] - m_rndElementGroupSize[spawnGroupIdx];
					if(indexLeftShift < 0)
					{
						indexLeftShift = m_randomOffsetHistorySize - Mathf.Abs(indexLeftShift);
					}
					
					if(!m_rndDistArray[spawnGroupIdx, indexLeftShift].HasValue)
					{
						newSpawnOffset = Random.Range(targetPE.spawnDistanceMinX, targetPE.spawnDistanceMaxX);
						m_rndDistArray[spawnGroupIdx, indexLeftShift] = newSpawnOffset;
						
						float rndAltYOffset = Random.Range(targetPE.randomYSpawnLowestAltOffset, targetPE.randomYSpawnHighestAltOffset);
						m_rndAltDistArray[spawnGroupIdx, indexLeftShift] = rndAltYOffset;
						if(targetPE.useRandomYSpawnAltOffset)
						{
							newYPos = targetPE.altSpawnAnchorY + (rndAltYOffset * targetPE.scale);
						}
					}
					else
					{
						newSpawnOffset = (float)m_rndDistArray[spawnGroupIdx, indexLeftShift];
						
						if(targetPE.useRandomYSpawnAltOffset)
						{
							newYPos = targetPE.altSpawnAnchorY + ((float)m_rndAltDistArray[spawnGroupIdx, indexLeftShift] * targetPE.scale);
						}
					}
					
					if(m_rndDistArrayIndex[spawnGroupIdx] == 0)
					{
						m_rndDistArrayIndex[spawnGroupIdx] = m_randomOffsetHistorySize;
					}
					else
					{
						m_rndDistArrayIndex[spawnGroupIdx]--;
					}
					
					if(m_rndAltDistArrayIndex[spawnGroupIdx] == 0)
					{
						m_rndAltDistArrayIndex[spawnGroupIdx] = m_randomOffsetHistorySize;
					}
					else
					{
						m_rndAltDistArrayIndex[spawnGroupIdx]--;
					}
					
				}
				else
				{
					newSpawnOffset = targetPE.spawnDistanceX;
				}
				
				edgeElement.parallaxObject.position = prevEdgeElement.parallaxObject.position - new Vector3(edgeElement.meshWidth + newSpawnOffset, 0, 0);
				if(edgeElement.useRandomYSpawnAltOffset)
				{
					edgeElement.parallaxObject.position = new Vector3( edgeElement.parallaxObject.position.x, newYPos, edgeElement.parallaxObject.position.z);
				}
				
				edgeElement.initialRightSpawnDistanceX = newSpawnOffset;
				
				targetPE.dupeChainObject.ShiftParallaxItemsRight();
				
				edgeElement = targetPE.dupeChainObject.GetEdgeParallaxElement(DupeChainHandle.Axis.X, direction);
				prevEdgeElement = targetPE.dupeChainObject.GetEdgeParallaxElement(DupeChainHandle.Axis.X, oppositeDirection);

				objectScreenPosX = m_mainCameraCam.WorldToScreenPoint(prevEdgeElement.parallaxObject.position).x;
			}
		}
	}
	
	void EstablishMaxDistance()
	{
		float[] zDepths = new float[m_parallaxElements.Length];
		for(int i = 0; i < m_parallaxElements.Length; i++)
		{
			if(m_parallaxElements[i].parallaxObject)
			{
				zDepths[i] = m_parallaxElements[i].parallaxObject.position.z - m_playerObj.transform.position.z;
			}
		}
		
		System.Array.Sort(zDepths);
		System.Array.Reverse (zDepths);
		m_maxDist = zDepths[0];
		m_maxDistDiv = 1 / m_maxDist;
	}
	
	private void SqueezeElementsArray()
	{
		List<EZParallaxObjectElement> tempElementArray = new List<EZParallaxObjectElement>(m_parallaxElements);
		IList<EZParallaxObjectElement> elemsToRemove = new List<EZParallaxObjectElement>();
		
		for(int i = 0; i < tempElementArray.Count; i++)
		{
			if(tempElementArray[i].parallaxObject == null || tempElementArray[i].parallaxObject.gameObject.activeSelf == false)
			{
				if(tempElementArray[i].parallaxObject.gameObject.activeSelf == false)
				{
					Debug.LogWarning("WARNING -- An inactive object named '" + tempElementArray[i].parallaxObject.name + "' is in the EZ Parallax parallaxing objects list. Removing from list. Please set object to active before initializing EZ Parallax, or use scripting API to assign the active object to EZ Parallax once it is active at runtime.");
				}
				elemsToRemove.Add(tempElementArray[i]);
			}
		}
		
		for(int i = 0; i < elemsToRemove.Count; i++)
		{
			tempElementArray.Remove (elemsToRemove[i]);
		}
		
		m_parallaxElements = tempElementArray.ToArray();
		
	}
	
	public void PurgeDupes()
	{
		List<EZParallaxObjectElement> tempElementArray = new List<EZParallaxObjectElement>(m_parallaxElements);
		IList<EZParallaxObjectElement> elemsToRemove = new List<EZParallaxObjectElement>();
		
		for(int i = 0; i < tempElementArray.Count; i++)
		{
			if(tempElementArray[i].isDupe)
			{
				elemsToRemove.Add(tempElementArray[i]);
			}
		}
		
		for(int i = 0; i < elemsToRemove.Count; i++)
		{
			tempElementArray.Remove (elemsToRemove[i]);
			GameObject.Destroy(elemsToRemove[i].parallaxObject.gameObject);
		}
		
		m_parallaxElements = tempElementArray.ToArray();
		SqueezeElementsArray();
	}
	
	//----------------------------------------------------------------------------------------------
	//  The following functions can be used by scripts at runtime to change the parallaxing elements
	//  or set the player/camera object. Calls into them would look something like this:
	//
	//       AssignPlayer(GameObject.Find("Player"), true); <--Camera function is just like this as well.
	//       AddNewParallaxingElement(GameObject.Find("BG6").transform);
	//	     RemoveParallaxingElement(GameObject.Find("BG2").transform);
	//----------------------------------------------------------------------------------------------
	
	//To manually initialize EZ Parallax without assigning a camera or player from code, call InitializeParallax(). The actual function can be found in the code above.
	
	//The second parameter here tells EZParallax if it should recalculate all the positions again, which should usually be "true" unless you are manually delaying parallax initialization.
	public void AssignPlayer(GameObject targetPlayerObj, bool doInit)
	{
		if(targetPlayerObj == null)
		{
			Debug.LogError("EZ Parallax: EZ Parallax AssignPlayer was passed a null object. Aborting function. Player was not assigned.");
			return;
		}
		m_playerObj = targetPlayerObj;
		if(doInit)
		{
			InitializeParallax();
		}
	}
	
	//Like the AssignPlayer function, the second parameter here tells EZParallax if it should recalculate all the positions again. You definitely want to have this set to "true" unless you are manually delaying the init.
	public void AssignCamera(GameObject targetCameraObj, bool doInit)
	{
		if(targetCameraObj.GetComponent<Camera>() == null)
		{
			Debug.LogError("EZ Parallax: EZ Parallax AssignCamera was passed a null object. Aborting function. Camera was not assigned.");
			return;
		}
		m_mainCamera = targetCameraObj;
		if(doInit)
		{
			InitializeParallax();
		}
	}
	
	//Use this function to add elements to the parallax list at runtime. Best used for elements that spawn on the fly. For other cases, using the editor inspector interface is encouraged.
	//To specify private speed scalars for your new object, add the x and y scalars as 2nd and 3rd arguments to the function call in your code.
	public EZParallaxObjectElement AddNewParallaxingElement(Transform targetElement)
	{
		return AddNewParallaxingElement(targetElement, 1, 1);
	}
	
	public EZParallaxObjectElement AddNewParallaxingElement(Transform targetElement, float privateSpeedScalarX, float privateSpeedScalarY)
	{
		for(int i = 0; i < m_parallaxElements.Length; i++)
		{
			if(m_parallaxElements[i].parallaxObject == targetElement)
			{
				Debug.LogWarning("EZ Parallax: WARNING -- AddNewParallaxingElement attempted to add an element that was arleady in the parallax list. Aborting.");
				return null;
			}
		}
		List<EZParallaxObjectElement> tempElementArray = new List<EZParallaxObjectElement>(m_parallaxElements);
		EZParallaxObjectElement newParallaxElement = new EZParallaxObjectElement(targetElement);
		newParallaxElement.privateParallaxSpeedScalarX = privateSpeedScalarX;
		newParallaxElement.privateParallaxSpeedScalarY = privateSpeedScalarY;
		tempElementArray.Add(newParallaxElement);
		m_parallaxElements = tempElementArray.ToArray();
		
		if( (targetElement.position.z - m_playerObj.transform.position.z) > m_maxDist)
		{
			m_maxDist = targetElement.position.z - m_playerObj.transform.position.z;
			m_maxDistDiv = 1 / m_maxDist;
		}
		
		if(m_initialized)
		{
			newParallaxElement.Initialize();
		}
		
		return newParallaxElement;
	}
	
	//If you remove an object from the parallaxing elements, make sure it's not visible in your camera view or it will break the parallaxing illusion for your player! :)
	//This function will only remove a single object. If you'd like to remove an object and all of its duplicates, use PurgeSingleDupeChain() found below.
	public void RemoveParallaxingElement(Transform targetElement)
	{
		int sourceElementCounter = 0;
		int newElementCounter = 0;
		EZParallaxObjectElement[] tempElementArray = new EZParallaxObjectElement[m_parallaxElements.Length - 1];
		foreach (EZParallaxObjectElement arrayElem in m_parallaxElements)
		{
			if(arrayElem.parallaxObject != targetElement)
			{
				tempElementArray[newElementCounter] = m_parallaxElements[sourceElementCounter];
				newElementCounter++;
			}
			sourceElementCounter++;
		}
		m_parallaxElements = tempElementArray;
	}
	
	//Use this function to set the infinite wrap settings on an object that you have dynamically added to the parallax list at runtime.
	public void SetElementWrapSettings(EZParallaxObjectElement targetElement, bool xWrapOn,  float spawnDistanceX)
	{
		targetElement.spawnsDuplicateOnX = xWrapOn;	
		targetElement.initialSpawnDistanceX = spawnDistanceX;
		
		if(m_initialized)
		{
			SpawnSingleElementDupes(targetElement, m_mainCameraCam, m_mainCameraCam.pixelWidth);
		}
	}
	
	//Use this function to set an object to be motorized after dynamically spawning it. If you wish to make the object wrap, be sure to set it to wrap FIRST, before applying the motorspeed.
	public void SetElementMotorized(EZParallaxObjectElement targetElement, float speed)
	{
		targetElement.isMotorized = true;
		targetElement.initialMotorSpeed = speed;
	}
	
	//This function will toggle motorization for an entire chain of objects if the target is part of a wrapping set of elements
	public void ToggleMotorization( Transform targetTransform, bool paused )
	{
		EZParallaxObjectElement targetElement = GetEZPObjectElementFromTransform(targetTransform);
		targetElement.motorizationPaused = paused;
		if(targetElement.dupeChainObject != null)
		{
			targetElement.dupeChainObject.motorIsPaused = paused;
		}
	}
	
	// Use this function to get the EZParallaxObjectElement from a transform. Generally, you shouldn't need this function if you just store the returned EZParallaxObjectElements returned when...
	// ...manually adding an element via AddNewParallaxingElement. Use this function for edge cases, or getting objects that you didn't manually add through code.
	public EZParallaxObjectElement GetEZPObjectElementFromTransform(Transform targetTransform)
	{
		EZParallaxObjectElement ezpElem = null;
		for(int i = 0; i < m_parallaxElements.Length; i++)
		{
			if(m_parallaxElements[i].parallaxObject == targetTransform)
			{
				ezpElem = m_parallaxElements[i];
				break;
			}
		}
		return ezpElem;
	}
	
	//Use this function to set an element to infinitely wrap with random distances between its duplicates. It automatically turns on x wrapping on the target object, so there is no need to manually set wrapping on the target before hand.
	//Optionally pass in the last bool and two floats (as seen in the overloaded function below) to turn on alternate axis (right now just Y axis) randomized spawning as well.
	public void SetElementRandomSpawn(EZParallaxObjectElement targetElement, float minRange, float maxRange)
	{
		SetElementRandomSpawn(targetElement, minRange, maxRange, false, 0.0f, 0.0f);
	}

	public void SetElementRandomSpawn(EZParallaxObjectElement targetElement, float minRange, float maxRange, bool useAltSpawn, float minAltRange, float maxAltRange)
	{
		targetElement.spawnsDuplicateOnX = true;
		targetElement.randomSpawnX = true;
		targetElement.initialSpawnDistanceMinX = minRange;
		targetElement.initialSpawnDistanceMaxX = maxRange;
		
		if(m_initialized)
		{			
			float?[,] tempArray = new float?[m_randomSpawnCtr + 1, m_randomOffsetHistorySize];
			System.Array.Copy(m_rndDistArray, tempArray, m_rndDistArray.Length);
			m_rndDistArray = new float?[m_randomSpawnCtr + 1, m_randomOffsetHistorySize];
			m_rndDistArray = tempArray;

			float?[,] tempAltArray = new float?[m_randomSpawnCtr + 1, m_randomOffsetHistorySize];
			System.Array.Copy(m_rndAltDistArray, tempAltArray, m_rndAltDistArray.Length);
			m_rndAltDistArray = new float?[m_randomSpawnCtr + 1, m_randomOffsetHistorySize];
			m_rndAltDistArray = tempAltArray;
			
			int[] tempGrpArray = new int[m_randomSpawnCtr + 1];
			System.Array.Copy(m_rndElementGroupSize, tempGrpArray, m_rndElementGroupSize.Length);
			m_rndElementGroupSize = new int[m_randomSpawnCtr + 1];
			m_rndElementGroupSize = tempGrpArray;
			
			int[] tempIdxArray = new int[m_randomSpawnCtr + 1];
			System.Array.Copy(m_rndDistArrayIndex, tempIdxArray, m_rndDistArrayIndex.Length);
			m_rndDistArrayIndex = new int[m_randomSpawnCtr + 1];
			m_rndDistArrayIndex = tempIdxArray;

			int[] tempAltIdxArray = new int[m_randomSpawnCtr + 1];
			System.Array.Copy(m_rndAltDistArrayIndex, tempAltIdxArray, m_rndAltDistArrayIndex.Length);
			m_rndAltDistArrayIndex = new int[m_randomSpawnCtr + 1];
			m_rndAltDistArrayIndex = tempAltIdxArray;

			if(useAltSpawn)
			{
				targetElement.useRandomYSpawnAltOffset = true;
				targetElement.randomYSpawnLowestAltOffset = minAltRange;
				targetElement.randomYSpawnHighestAltOffset = maxAltRange;
			}
			
			SpawnSingleElementDupes(targetElement, m_mainCameraCam, m_mainCameraCam.pixelWidth);
		}
	}

	//If the player changes its Z position, call this function to update the depth values for the parallaxing elements. If left alone after changing the position of the player on the Z axis, you may...
	//..experience strange parallaxing behavior, so be sure to use this.
	//This function doesn't need to be called if a call to InitializeParallax is pending, as it will update the depth values as well.
	public void UpdatePlayerZ()
	{
		EstablishMaxDistance();		
	}
	
	//Use this function to remove a SINGLE chain of elements spawned from a single EZP element. I.e., maybe you have wrapping clouds, and you want to remove all of them from the scene. Pass in any...
	//...of your cloud element's transforms, and all the sibling clouds generated by EZP will be removed from the scene, along with the original transform/elemnent being passed.
	public void PurgeSingleDupeChain(Transform targetTransform)
	{
		EZParallaxObjectElement targetElement = GetEZPObjectElementFromTransform(targetTransform);

		if(targetElement == null)
		{
			Debug.LogWarning("EZ Parallax: The call to PurgeSingleDupeChain could not find the target transform. Aborting purge.");
			return;
		}
		
		if(targetElement.randomSpawnX)
		{
			int spawnGroupIdx = targetElement.spawnGroupIndex;
			
			//Make all storage and history items related to this randomized object chain 0, so that people viewing the data will be able to identify cleared slots in the array, while still keeping array structure intact.
			for(int i = 0; i < m_rndDistArray.GetLength(0); i++)
			{
				m_rndDistArray[spawnGroupIdx, i] = 0.0f;
				m_rndAltDistArray[spawnGroupIdx, i] = 0.0f;
			}
			
			m_rndDistArrayIndex[spawnGroupIdx] = 0;
			m_rndAltDistArrayIndex[spawnGroupIdx] = 0;
			m_rndElementGroupSize[spawnGroupIdx] = 0;
		}
		
		List<EZParallaxObjectElement> tempElementArray = new List<EZParallaxObjectElement>(m_parallaxElements);
		IList<EZParallaxObjectElement> elemsToRemove = new List<EZParallaxObjectElement>(targetElement.dupeChainObject.GetElementList(targetElement));
		
		//Purge.
		for(int i = 0; i < elemsToRemove.Count; i++)
		{
			tempElementArray.Remove (elemsToRemove[i]);
			GameObject.Destroy(elemsToRemove[i].parallaxObject.gameObject);
		}
		
		DupeChainHandle[] tempHandleArray = new DupeChainHandle[m_dupeChainHandles.Length - 1];
		int tempIdxCtr = 0;
		bool foundObj = false;
		for(int i = 0; i < m_dupeChainHandles.Length; i++)
		{
			if(m_dupeChainHandles[i].targetGameObject.transform.FindChild(targetTransform.gameObject.name) != null)
			{
				GameObject.Destroy(m_dupeChainHandles[i].targetGameObject);
				m_dupeChainHandles[i] = null;
				foundObj = true;
			}
			else
			{
				tempHandleArray[tempIdxCtr] = m_dupeChainHandles[i];
				tempIdxCtr++;
			}
		}
		
		if(foundObj)
		{
			m_dupeChainHandles = tempHandleArray;
		}
		
		m_parallaxElements = tempElementArray.ToArray();
		SqueezeElementsArray();
	}
	
	//Use this function to manually toggle dolly vs traditional zoom during runtime. Can be used effectively to take advantage of both effects!
	public void ToggleDollyZoom(bool newToggle)
	{
		m_enableDollyZoom = newToggle;
	}
}
