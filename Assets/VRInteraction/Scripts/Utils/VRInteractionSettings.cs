using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace VRInteraction
{
    public class VRInteractionSettings
    {
        private static VRInteractionSettings _instance;
        public static VRInteractionSettings instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new VRInteractionSettings();
                    _instance.Load();
                }
                return _instance;
            }
        }

        public class SerializableSettings
        {
            public VRInteractableItem.HoldType holdType = VRInteractableItem.HoldType.FIXED_POSITION;
            public float followForce = 1f;
            public float throwBoost = 1f;
            public bool toggleToPickup = false;
            public bool useBreakDistance = false;
            public float breakDistance = 0.1f;
            public float interactionDistance = 0.1f;
            public bool limitAcceptedAction = false;
            public List<string> acceptedActions = new List<string>();
        }

        public SerializableSettings settings;

        virtual protected void Load()
        {
            string jsonSettings = "";
            jsonSettings = PlayerPrefs.GetString("VRInteractionSettings", "");
            if (string.IsNullOrEmpty(jsonSettings))
            {
                TextAsset textSettings = Resources.Load<TextAsset>("VRInteractionSettings");
                if (textSettings != null) jsonSettings = textSettings.text;
            }
            if (string.IsNullOrEmpty(jsonSettings))
            {
                settings = new SerializableSettings();
                //Add default values or settings here
                return;
            }
            settings = (SerializableSettings)JsonUtility.FromJson(jsonSettings, typeof(SerializableSettings));
        }


        virtual public void Save()
        {
            string jsonSettings = JsonUtility.ToJson(settings);
            PlayerPrefs.SetString("VRInteractionSettings", jsonSettings);
            PlayerPrefs.Save();
#if UNITY_EDITOR
            string path = "Assets/VRInteraction/Resources/";
            string fileName = "VRInteractionSettings.txt";

            Directory.CreateDirectory(path);
            StreamWriter writer = new StreamWriter(path + fileName, false);
            writer.Write(jsonSettings);
            writer.Close();
            AssetDatabase.ImportAsset(path + fileName);
#endif
        }

    }
}