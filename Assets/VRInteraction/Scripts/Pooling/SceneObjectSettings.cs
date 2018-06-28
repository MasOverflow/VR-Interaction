using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRInteraction
{
	public class SceneObjectSettings : MonoBehaviour 
	{
		[System.Serializable]
		public class PoolObject
		{
			public GameObject prefab;
			public int prepool;
			public bool delayDisable;
		}

		public List<PoolObject> poolObjects;

		void Start()
		{
			foreach(PoolObject poolObject in poolObjects) PoolingManager.instance.CreatePoolObjectGroup(poolObject.prefab, poolObject.prepool, poolObject.delayDisable);
		}
	}
}