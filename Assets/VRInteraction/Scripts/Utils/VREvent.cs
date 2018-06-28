//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Static event system, call Listen and give it a method to call and when
// Send is called with the same name all listener methods will be called.
// Used for tracking objects being picked up or dropped.
// But can be used to listen for any kind of event really, sky's the limit!
//
//===================Contact Email: Sam@MassGames.co.uk===========================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VREvent : MonoBehaviour
{	
	[System.Serializable]
	public class ObjectEvent : UnityEvent<object[]>
	{}

	private Dictionary <string, ObjectEvent> eventDictionary;

	private static VREvent eventManager;

	public static VREvent instance
	{
		get
		{
			if (eventManager == null)
			{
				GameObject eventMangerObj = new GameObject("EventManager");
				eventManager = eventMangerObj.AddComponent<VREvent>();
				eventManager.Init();
			}
			return eventManager;
		}
	}

	void Init ()
	{
		if (eventDictionary == null) eventDictionary = new Dictionary<string, ObjectEvent>();
	}

	public static void Listen (string eventName, UnityAction<object[]> listener)
	{
		ObjectEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.AddListener (listener);
		} 
		else
		{
			thisEvent = new ObjectEvent();
			thisEvent.AddListener (listener);
			instance.eventDictionary.Add (eventName, thisEvent);
		}
	}

	public static void Remove (string eventName, UnityAction<object[]> listener)
	{
		if (eventManager == null) return;
		ObjectEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.RemoveListener (listener);
		}
	}

	public static void Send (string eventName, object[] param)
	{
		ObjectEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.Invoke(param);
		}
	}

	/*public delegate void Handler(params object[] args);

	public static void Listen(string message, Handler action)
	{
		var actions = listeners[message] as Handler;
		if (actions != null)
		{
			listeners[message] = actions + action;
		}
		else
		{
			listeners[message] = action;
		}
	}

	public static void Remove(string message, Handler action)
	{
		var actions = listeners[message] as Handler;
		if (actions != null)
		{
			listeners[message] = actions - action;
		}
	}

	public static void Send(string message, params object[] args)
	{
		var actions = listeners[message] as Handler;
		if (actions != null)
		{
			actions(args);
		}
	}

	public static bool Contains(string message, Handler action)
	{
		var actions = listeners[message] as Handler;
		if (actions != null)
		{
			var delegates = actions.GetInvocationList();
			foreach(var dele in delegates)
			{
				if (dele == action)
					return true;
			}
		}
		return false;
	}

	private static Hashtable listeners = new Hashtable();*/


}
