using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnectionObject : NetworkBehaviour 
{
	public GameObject playerRigPrefab;

	private GameObject _singlePlayerInstance;

	void Start()
	{
		if (!isLocalPlayer) return;

		if (_singlePlayerInstance == null) _singlePlayerInstance = Camera.main.transform.root.gameObject;
		_singlePlayerInstance.SetActive(false);

		CmdSpawnPlayer();

		if (isServer)
		{
			//	Begin world
			NetworkedItemSpawner[] itemSpawners = FindObjectsOfType<NetworkedItemSpawner>();
			foreach(NetworkedItemSpawner itemSpawner in itemSpawners)
			{
				GameObject newItem = itemSpawner.SpawnItem();
				if (newItem != null) NetworkServer.Spawn(newItem);
			}
		}
	}

	void OnDestroy()
	{
		if (!isLocalPlayer) return;
		if (_singlePlayerInstance != null) _singlePlayerInstance.SetActive(true);
	}

	[Command]
	void CmdSpawnPlayer()
	{
		GameObject go = Instantiate<GameObject>(playerRigPrefab);
		NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
	}

	//[Command]
	//void SpawnNetoewk
}
