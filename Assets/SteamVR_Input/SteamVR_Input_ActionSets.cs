// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.1433
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace Valve.VR
{
    using System;
    using UnityEngine;
    
    
    public partial class SteamVR_Actions
    {
        
        private static SteamVR_Input_ActionSet_VRInteraction p_VRInteraction;
        
        public static SteamVR_Input_ActionSet_VRInteraction VRInteraction
        {
            get
            {
                return SteamVR_Actions.p_VRInteraction.GetCopy <SteamVR_Input_ActionSet_VRInteraction>();
            }
        }
        
        private static void StartPreInitActionSets()
        {
            SteamVR_Actions.p_VRInteraction = ((SteamVR_Input_ActionSet_VRInteraction)(SteamVR_ActionSet.Create <SteamVR_Input_ActionSet_VRInteraction>("/actions/VRInteraction")));
            Valve.VR.SteamVR_Input.actionSets = new Valve.VR.SteamVR_ActionSet[]
            {
                    SteamVR_Actions.VRInteraction};
        }
    }
}
