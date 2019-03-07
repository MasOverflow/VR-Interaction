Base VRInteraction asset.

To setup the 
To setup for SteamVR:
Drag the '[CameraRig]' prefab from the prefabs folder in SteamVR into the scene, on each controller object click 'Add Component' and search for 
'VR Interactor'.
To Setup for Oculus Native:
Create a new empty gameobject in the scene (optionally make sure it is on the floor), drag the 'OVRCameraRig' prefab as a child of the empty gameobject 
(optionally make sure 'Tracking Origin Type' is set to 'Floor Level' on the OVRManager component), drag the 'LocalAvatar' prefab as a child of the empty gameobject, 
on either both controllers or both hand objects click 'Add Component' and search for 'VR Interactor'.

There are Items setup in the ExampleScene, if you can pick them up then everything is working.

Videos:
VR Interaction Demo: https://www.youtube.com/watch?v=nBkRuxQSBXg
Setting up the player rig tutorial: https://www.youtube.com/watch?v=f81boZJVxnM

FAQ:
Q: Why does the error "Some objects were not cleaned up when closing the scene. (Did you spawn new GameObjects from OnDestroy?)
A: This happens when you stop the editor while holding a VRInteracableItem, it's caused by creating the drop event, which is called by the item's OnDisable which is called when Destroy is called. This should only occur in the editor when you stop play mode.