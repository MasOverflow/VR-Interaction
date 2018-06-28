using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRInteraction
{
	public class PoolingManager : MonoBehaviour
	{
		private static PoolingManager _instance;
		public static PoolingManager instance
		{
			get
			{
				if (_instance == null)
				{
					GameObject poolObject = new GameObject("PoolingObject");
					_instance = poolObject.AddComponent<PoolingManager>();
				}
				return _instance;
			}
		}
			
		private Dictionary<GameObject, List<GameObject>> instances = new Dictionary<GameObject, List<GameObject>>();
		private Vector3 poolingWorldPosition = new Vector3(0f, -5000f, 0f);
		private SceneObjectSettings _sceneSettings;

		private void Awake() 
		{
			Init();
		}

		private void Init()
		{
			_sceneSettings = FindObjectOfType<SceneObjectSettings>();
			if (_sceneSettings != null && (!_sceneSettings.gameObject.activeInHierarchy || !_sceneSettings.enabled)) _sceneSettings = null;

			//GameObject[] objectsArray = Resources.LoadAll<GameObject>("objects");
			/*foreach(GameObject tempObject in objectsArray)
			{
				PrefabObject newPrefabObject = new PrefabObject();
				if (_sceneSettings)
				{
					for(int i=0 ; i<_sceneSettings.prepoolList.Count ; i++)
					{
						string prepoolName = _sceneSettings.prepoolList[i];
						if (tempObject.name != prepoolName) continue;
						newPrefabObject.poolObject = true;
						newPrefabObject.prepool = _sceneSettings.prepool[i];
						break;
					}
				}
				newPrefabObject.prefab = tempObject;
				prefabs.Add(newPrefabObject);
			}

			foreach(PrefabObject prefabObject in prefabs)
			{
				CreatePoolObjectGroup(prefabObject);
			}*/
		}

		IEnumerator DelayedDisable(GameObject instance)
		{
			yield return null;
			yield return null;
			instance.transform.SetParent(transform);
			instance.SetActive(false);
		}

		public void CreatePoolObjectGroup(GameObject prefab, int prepool, bool requiresDelayedDisable = false)
		{
			if (instances.ContainsKey(prefab)) return; //Already have object group for this prefab

			instances.Add(prefab, new List<GameObject>());
			for (int i=0; i<prepool; i++) CreateNewPoolObject(prefab, requiresDelayedDisable);
		}

		private GameObject CreateNewPoolObject(GameObject prefab, bool requiresDelayedDisable)
		{
			if (!instances.ContainsKey(prefab)) instances.Add(prefab, new List<GameObject>()); //Pass this the currentInstances list, to see if it's still a reference
			List<GameObject> currentInstances = null;
			instances.TryGetValue(prefab, out currentInstances);
			GameObject instance = (GameObject)Instantiate(prefab, poolingWorldPosition, Quaternion.identity, transform);

			if (requiresDelayedDisable) StartCoroutine(DelayedDisable(instance));
			else instance.SetActive(false);
			currentInstances.Add(instance);
			return instance;
		}

		/// <summary>
		/// Returns an instance of a prefab. Instance will be disabled so you can change any variables before enabling.
		/// Make sure to SetActive(true) the instance in the same frame or call RemoveInstanceFromObjectPool. Otherwise this
		/// instance could be given to the next GetInstance request of the same prefab.
		/// </summary>
		/// <returns>The instance.</returns>
		/// <param name="prefab">Prefab.</param>
		public GameObject GetInstance(GameObject prefab)
		{
			List<GameObject> currentInstances = null;
			instances.TryGetValue(prefab, out currentInstances);
			if (currentInstances == null)
			{
				CreateNewPoolObject(prefab, false);
				instances.TryGetValue(prefab, out currentInstances);
			} else if (currentInstances.Count != 0) currentInstances.RemoveAll(item => item == null);

			GameObject goodInstance = null;
			foreach(GameObject instance in currentInstances)
			{
				if (!instance.activeSelf)
				{
					goodInstance = instance;
					break;
				}
			}
			if (goodInstance == null) goodInstance = CreateNewPoolObject(prefab, false);
			return goodInstance;
		}

		public void SetupDestroyInToReset(GameObject instance, bool addDestroyIn = false, float defaultTime = 5f)
		{
			DestroyIn destroyIn = instance.GetComponent<DestroyIn>();
			if (addDestroyIn && destroyIn == null)
			{
				destroyIn = instance.AddComponent<DestroyIn>();
				destroyIn.seconds = defaultTime;
			} else if (destroyIn == null) return;
			destroyIn.disableOnly = true;
			destroyIn.reParent = transform;
		}

		/// <summary>
		/// Deactivates Object and calls Reset method on all scripts, optionally delay in seconds.
		/// </summary>
		/// <param name="instance">Instance to destroy.</param>
		/// <param name="timeToDestroy">Time to destroy.</param>
		public void DestroyPoolObject(GameObject instance, float timeToDestroy = -1f)
		{
			if (instance == null) return;
			if (timeToDestroy > 0f)
			{
				StartCoroutine(DelayDestroy(instance, timeToDestroy));
				return;
			}
			instance.BroadcastMessage("Reset", SendMessageOptions.DontRequireReceiver);
			instance.transform.SetParent(transform);
			instance.SetActive(false);
		}
			
		private IEnumerator DelayDestroy(GameObject instance, float timeToDestroy)
		{
			yield return new WaitForSeconds(timeToDestroy);
			DestroyPoolObject(instance);
		}

		public void RemoveInstanceFromObjectPool(GameObject instance)
		{
			if (instance == null) return;
			foreach(GameObject prefab in instances.Keys)
			{
				List<GameObject> currentInstances = null;
				instances.TryGetValue(prefab, out currentInstances);
				if (currentInstances == null || currentInstances.Count == 0 || !currentInstances.Contains(instance)) continue;

				currentInstances.Remove(instance);
				return;
			}
		}

		//public GameObject GetPrefabRef(string prefabName)
		//{
		//foreach(GameObject keyPrefab in instances.Keys)
		//{
		//if (keyPrefab.name != prefabName) continue;
		//return keyPrefab;
		//}
		//return null;
		//}
	}
}