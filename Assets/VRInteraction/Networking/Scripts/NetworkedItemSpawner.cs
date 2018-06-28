using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedItemSpawner : NetworkBehaviour
{
	public GameObject prefab;

	/*public override void OnStartClient ()
	{
		base.OnStartClient ();
		Debug.Log("register prefab " + prefab.name);
		ClientScene.RegisterPrefab(prefab, NetworkHash128.Parse(prefab.name));
	}*/

	public GameObject SpawnItem()
	{
		GameObject newInstance = Instantiate<GameObject>(prefab, transform.position, transform.rotation);

		/*NetworkIdentity newIdentity = newInstance.GetComponent<NetworkIdentity>();
		if (newIdentity == null) newIdentity = newInstance.AddComponent<NetworkIdentity>();

		NetworkTransform newTransform = newIdentity.GetComponent<NetworkTransform>();
		if (newTransform == null) newTransform = newInstance.AddComponent<NetworkTransform>();*/

		return newInstance;
	}
}
