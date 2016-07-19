/////////////////////////////////////////////////////////////////////////////////
//
//	vp_Utility.cs
//	© VisionPunk. All Rights Reserved.
//	https://twitter.com/VisionPunk
//	http://www.visionpunk.com
//
//	description:	miscellaneous utility functions
//
/////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Diagnostics;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

public static class vp_Utility
{


	/// <summary>
	/// Cleans non numerical values (NaN) from a float by
	/// retaining a previous property value. If 'prevValue' is
	/// omitted, the NaN will be replaced by '0.0f'.
	/// </summary>
	public static float NaNSafeFloat(float value, float prevValue = default(float))
	{

		value = double.IsNaN(value) ? prevValue : value;
		return value;

	}


	/// <summary>
	/// Cleans non numerical values (NaN) from a Vector2 by
	/// retaining a previous property value. If 'prevVector' is
	/// omitted, the NaN will be replaced by '0.0f'.
	/// </summary>
	public static Vector2 NaNSafeVector2(Vector2 vector, Vector2 prevVector = default(Vector2))
	{

		vector.x = double.IsNaN(vector.x) ? prevVector.x : vector.x;
		vector.y = double.IsNaN(vector.y) ? prevVector.y : vector.y;

		return vector;

	}


	/// <summary>
	/// Cleans non numerical values (NaN) from a Vector3 by
	/// retaining a previous property value. If 'prevVector' is
	/// omitted, the NaN will be replaced by '0.0f'.
	/// </summary>
	public static Vector3 NaNSafeVector3(Vector3 vector, Vector3 prevVector = default(Vector3))
	{

		vector.x = double.IsNaN(vector.x) ? prevVector.x : vector.x;
		vector.y = double.IsNaN(vector.y) ? prevVector.y : vector.y;
		vector.z = double.IsNaN(vector.z) ? prevVector.z : vector.z;

		return vector;

	}


	/// <summary>
	/// Cleans non numerical values (NaN) from a Quaternion by
	/// retaining a previous property value. If 'prevQuaternion'
	/// is omitted, the NaN will be replaced by '0.0f'.
	/// </summary>
	public static Quaternion NaNSafeQuaternion(Quaternion quaternion, Quaternion prevQuaternion = default(Quaternion))
	{

		quaternion.x = double.IsNaN(quaternion.x) ? prevQuaternion.x : quaternion.x;
		quaternion.y = double.IsNaN(quaternion.y) ? prevQuaternion.y : quaternion.y;
		quaternion.z = double.IsNaN(quaternion.z) ? prevQuaternion.z : quaternion.z;
		quaternion.w = double.IsNaN(quaternion.w) ? prevQuaternion.w : quaternion.w;

		return quaternion;

	}


	/// <summary>
	/// This can be used to snap individual super-small property
	/// values to zero, for avoiding some floating point issues.
	/// </summary>
	public static Vector3 SnapToZero(Vector3 value, float epsilon = 0.0001f)
	{

		value.x = (Mathf.Abs(value.x) < epsilon) ? 0.0f : value.x;
		value.y = (Mathf.Abs(value.y) < epsilon) ? 0.0f : value.y;
		value.z = (Mathf.Abs(value.z) < epsilon) ? 0.0f : value.z;
		return value;

	}


	/// <summary>
	/// Zeroes the y property of a Vector3, for some cases where you want to
	/// make 2D physics calculations.
	/// </summary>
	public static Vector3 HorizontalVector(Vector3 value)
	{

		value.y = 0.0f;
		return value;

	}


	/// <summary>
	/// Performs a stack trace to see where things went wrong
	/// for error reporting.
	/// </summary>
	public static string GetErrorLocation(int level = 1)
	{

		StackTrace stackTrace = new StackTrace();
		string result = "";
		string declaringType = "";

		for (int v = stackTrace.FrameCount - 1; v > level; v--)
		{
			if (v < stackTrace.FrameCount - 1)
				result += " --> ";
			StackFrame stackFrame = stackTrace.GetFrame(v);
			if (stackFrame.GetMethod().DeclaringType.ToString() == declaringType)
				result = "";	// only report the last called method within every class
			declaringType = stackFrame.GetMethod().DeclaringType.ToString();
			result += declaringType + ":" + stackFrame.GetMethod().Name;
		}

		return result;

	}


	/// <summary>
	/// Returns the 'syntax style' formatted version of a type name.
	/// for example: passing 'System.Single' will return 'float'.
	/// </summary>
	public static string GetTypeAlias(Type type)
	{

		string s = "";

		if (!m_TypeAliases.TryGetValue(type, out s))
			return type.ToString();

		return s;

	}


	/// <summary>
	/// Dictionary of type aliases for error messages.
	/// </summary>
	private static readonly Dictionary<Type, string> m_TypeAliases = new Dictionary<Type, string>()
	{

		{ typeof(void), "void" },
		{ typeof(byte), "byte" },
		{ typeof(sbyte), "sbyte" },
		{ typeof(short), "short" },
		{ typeof(ushort), "ushort" },
		{ typeof(int), "int" },
		{ typeof(uint), "uint" },
		{ typeof(long), "long" },
		{ typeof(ulong), "ulong" },
		{ typeof(float), "float" },
		{ typeof(double), "double" },
		{ typeof(decimal), "decimal" },
		{ typeof(object), "object" },
		{ typeof(bool), "bool" },
		{ typeof(char), "char" },
		{ typeof(string), "string" },
		{ typeof(UnityEngine.Vector2), "Vector2" },
		{ typeof(UnityEngine.Vector3), "Vector3" },
		{ typeof(UnityEngine.Vector4), "Vector4" }

	};
	

	/// <summary>
	/// Activates or deactivates a gameobject for any Unity version.
	/// </summary>
	public static void Activate(GameObject obj, bool activate = true)
	{

#if UNITY_3_5
		obj.SetActiveRecursively(activate);
#else
		obj.SetActive(activate);
#endif

	}


	/// <summary>
	/// Returns active status of a gameobject for any Unity version.
	/// </summary>
	public static bool IsActive(GameObject obj)
	{

#if UNITY_3_5
		return obj.active;
#else
		return obj.activeSelf;
#endif

	}
	

	/// <summary>
	/// Plays a random sound from a list, with a random pitch.
	/// </summary>
	public static void PlayRandomSound(AudioSource audioSource, List<AudioClip> sounds, Vector2 pitchRange)
	{

		if (audioSource == null)
			return;

		if (sounds == null || sounds.Count == 0)
			return;

		AudioClip soundToPlay = sounds[UnityEngine.Random.Range(0, sounds.Count)];

		if (soundToPlay == null)
			return;

		if (pitchRange == Vector2.one)
			audioSource.pitch = Time.timeScale;
		else
			audioSource.pitch = UnityEngine.Random.Range(pitchRange.x, pitchRange.y) * Time.timeScale;

		audioSource.PlayOneShot(soundToPlay);

	}


	/// <summary>
	/// Plays a random sound from a list.
	/// </summary>
	public static void PlayRandomSound(AudioSource audioSource, List<AudioClip> sounds)
	{
		PlayRandomSound(audioSource, sounds, Vector2.one);
	}


	/// <summary>
	/// Randomizes the order of the objects in the specified list.
	/// </summary>
	public static void RandomizeList<T>(this List<T> list)
	{

		int size = list.Count;

		for (int i = 0; i < size; i++)
		{
			int indexToSwap = UnityEngine.Random.Range(i, size);
			T oldValue = list[i];
			list[i] = list[indexToSwap];
			list[indexToSwap] = oldValue;
		}

	}


	/// <summary>
	/// Returns a random object from a list.
	/// </summary>
	public static T RandomObject<T>(this List<T> list)
	{

		List<T> newList = new List<T>();
		newList.AddRange(list);
		newList.RandomizeList();
		return newList.FirstOrDefault();

	}
	
	
	/// <summary>
	/// Returns a list of the specified child components
	/// </summary>
	public static List<T> ChildComponentsToList<T>( this Transform t ) where T : Component
	{

		return t.GetComponentsInChildren<T>().ToList();

	}
	
	
	/// <summary>
	/// Replacement for Object.Instantiate in order to utilize pooling
	/// </summary>
	public static UnityEngine.Object Instantiate( UnityEngine.Object original )
    {
    
    	return vp_Utility.Instantiate(original, Vector3.zero, Quaternion.identity);
    
    }
    
    
    /// <summary>
	/// Replacement for Object.Instantiate in order to utilize pooling
	/// </summary>
    public static UnityEngine.Object Instantiate( UnityEngine.Object original, Vector3 position, Quaternion rotation )
    {
    
    	if(vp_PoolManager.Instance == null || !vp_PoolManager.Instance.enabled)
    		return GameObject.Instantiate(original, position, rotation);
    	else
    		return vp_GlobalEventReturn<UnityEngine.Object, Vector3, Quaternion, UnityEngine.Object>.Send("vp_PoolManager Instantiate", original, position, rotation);
    
    }
    
    
    /// <summary>
	/// Replacement for Object.Destroy in order to utilize pooling
	/// </summary>
    public static void Destroy( UnityEngine.Object obj )
    {
    
    	vp_Utility.Destroy( obj, 0 );
    
    }
    
    
    /// <summary>
	/// Replacement for Object.Destroy in order to utilize pooling
	/// </summary>
    public static void Destroy( UnityEngine.Object obj, float t )
    {
    
    	if(vp_PoolManager.Instance == null || !vp_PoolManager.Instance.enabled)
	    	UnityEngine.Object.Destroy( obj, t );
	    else
	    	vp_GlobalEvent<UnityEngine.Object, float>.Send("vp_PoolManager Destroy", obj, t);
    
    }


}

