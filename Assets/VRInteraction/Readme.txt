Base VRInteraction asset.

For tutorials check the youtube: https://www.youtube.com/channel/UCfMxMaCkR3TlDb0Bu7BkdEA
Refer to the Setup Guide found in Assets/VRInteraction/Docs for more setup info.

To setup the 
To setup for SteamVR:
Drag the '[CameraRig]' prefab from the prefabs folder in SteamVR into the scene, on each controller object click 'Add Component' and search for 
'VR Interactor'. Refer to the setup guide SteamVR 2.0 section for setting SteamVR Inputs.
For setting up SteamVR 2.0 there are some premade actions in Assets/VRInteraction/SteamVR_Actions, if you copy and paste them to the root folder (That's where the Assembly .csproj files are)
then open the Window->SteamVR Input window, you should be able to see them, click Save and generate and they should appear in the VRInput script and should be assigned to bindings already
if not then you can assign them in the binding UI page.

To Setup for Oculus Native:
Drag the 'OVRCameraRig' prefab into the scene (optionally make sure 'Tracking Origin Type' is set to 'Floor Level' on the OVRManager component),
on either or both hand anchors click 'Add Component' and search for 'VR Interactor'.

There are Items setup in the ExampleScene, if you can pick them up then everything is working.


FAQ:
Q:Can't pick up items or can't see controllers/hands
A: In the oculus update that supports Oculus Go it defaulted the controller so every project with AndroidManifest no explicitly defined for Quest
suddenly switched to Go controllers. It is necessary to configure the manifest.
Otherwise likely an issue with the player rig. if SteamVR grab the [CameraRig] prefab and attach the VRInteractor component to each controller
make sure to setup any actions you're using in SteamVR Input bindings. If Oculus grab the OVRCameraRig prefab from the Oculus Integration
asset, attach the VRInteractor component to the left and right hand anchors (if you can't see controllers add the OVRControllerPrefab as
a child of each hand anchor and setup as per Oculus docs) Make sure the keys have the right actions, add new actions in the Edit Actions foldout
on the VRInteractor script
Q: Why does the error "Some objects were not cleaned up when closing the scene. (Did you spawn new GameObjects from OnDestroy?)"
A: This happens when you stop the editor while holding a VRInteracableItem,
it's caused by creating the drop event, which is called by the item's OnDisable
which is called when Destroy is called. This should only occur in the editor when you stop play mode.
