using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace WingProcedural
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class StaticWingGlobals : MonoBehaviour
    {
        public static List<WingTankConfiguration> wingTankConfigurations = new List<WingTankConfiguration>();

        public static Shader wingShader;

        public static GameObject handlesRoot, normalHandles, ctrlSurfHandles, hingeIndicator,
                                 handleLength, handleWidthRootFront, handleWidthRootBack, handleWidthTipFront, handleWidthTipBack,
                                 handleLeadingRoot, handleLeadingTip, handleTrailingRoot, handleTrailingTip;
        public static GameObject ctrlHandleLength1, ctrlHandleLength2,
                                 ctrlHandleRootWidthOffset, ctrlHandleTipWidthOffset,
                                 ctrlHandleTrailingRoot, ctrlHandleTrailingTip;

        private static string _bundlePath;
        public string BundlePath
        {
            get
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.OSXPlayer:
                        return _bundlePath + Path.DirectorySeparatorChar + "pwings_macosx.bundle";
                    case RuntimePlatform.WindowsPlayer:
                        return _bundlePath + Path.DirectorySeparatorChar + "pwings_windows.bundle";
                    case RuntimePlatform.LinuxPlayer:
                        return _bundlePath + Path.DirectorySeparatorChar + "pwings_linux.bundle";
                    default:
                        return _bundlePath + Path.DirectorySeparatorChar + "pwings_windows.bundle";
                }
            }
        }

		public static bool loadingAssets = false;
		public static StaticWingGlobals Instance;

        private void Awake()
        {
            _bundlePath = KSPUtil.ApplicationRootPath + "GameData" +
                                                    Path.DirectorySeparatorChar +
                                                    "B9_Aerospace_ProceduralWings" + Path.DirectorySeparatorChar + "AssetBundles";

			if (Instance != null) Destroy(Instance);
			Instance = this;
        }

        public void Start()
        {
            foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("ProceduralWingFuelSetups"))
            {
                ConfigNode[] fuelNodes = node.GetNodes("FuelSet");
                for (int i = 0; i < fuelNodes.Length; ++i)
                {
                    wingTankConfigurations.Add(new WingTankConfiguration(fuelNodes[i]));
                }
            }
            Debug.Log("[B9PW] start bundle load process");

            StartCoroutine(LoadBundleAssets());
        }

        public IEnumerator LoadBundleAssets()
        {
            Debug.Log("[B9PW] Aquiring bundle data");
            AssetBundle shaderBundle = AssetBundle.LoadFromFile(BundlePath);

            if (shaderBundle != null)
            {
                Shader[] objects = shaderBundle.LoadAllAssets<Shader>();
                for (int i = 0; i < objects.Length; ++i)
                {
                    if (objects[i].name == "KSP/Specular Layered")
                    {
                        wingShader = objects[i];
                        Debug.Log($"[B9PW] Wing shader \"{wingShader.name}\" loaded. Shader supported? {wingShader.isSupported}");
                    }
                }

                #region Handle gizmos---CarnationRED 2020-6
                GameObject[] objects1 = shaderBundle.LoadAllAssets<GameObject>();
                for (int i = 0; i < objects1.Length; i++)
                {
                    GameObject item = objects1[i];
                    if (item.name.Equals("handlesRoot"))
                    {
                        handlesRoot = Instantiate(item);
                        break;
                    }
                }
                if (handlesRoot)
                {
                    handlesRoot.SetActive(false);
                    DontDestroyOnLoad(handlesRoot);

                    normalHandles = handlesRoot.transform.Find("Normal").gameObject;
                    ctrlSurfHandles = handlesRoot.transform.Find("CtrlSurf").gameObject;
                    hingeIndicator = handlesRoot.transform.Find("RotateAxis").gameObject;
                    foreach (Transform obj in normalHandles.transform)
                        obj.gameObject.AddComponent<EditorHandle>();

                    foreach (Transform obj in ctrlSurfHandles.transform) 
                        obj.gameObject.AddComponent<EditorHandle>();

                    handleLength = normalHandles.transform.Find("handleLength").gameObject;
                    handleWidthRootFront = normalHandles.transform.Find("handleWidthRootFront").gameObject;
                    handleWidthRootBack = normalHandles.transform.Find("handleWidthRootBack").gameObject;
                    handleWidthTipFront = normalHandles.transform.Find("handleWidthTipFront").gameObject;
                    handleWidthTipBack = normalHandles.transform.Find("handleWidthTipBack").gameObject;
                    handleLeadingRoot = normalHandles.transform.Find("handleLeadingRoot").gameObject;
                    handleLeadingTip = normalHandles.transform.Find("handleLeadingTip").gameObject;
                    handleTrailingRoot = normalHandles.transform.Find("handleTrailingRoot").gameObject;
                    handleTrailingTip = normalHandles.transform.Find("handleTrailingTip").gameObject;

                    ctrlHandleLength1 = ctrlSurfHandles.transform.Find("ctrlHandleLength1").gameObject;
                    ctrlHandleLength2 = ctrlSurfHandles.transform.Find("ctrlHandleLength2").gameObject;
                    ctrlHandleRootWidthOffset = ctrlSurfHandles.transform.Find("ctrlHandleRootWidthOffset").gameObject;
                    ctrlHandleTipWidthOffset = ctrlSurfHandles.transform.Find("ctrlHandleTipWidthOffset").gameObject;
                    ctrlHandleTrailingRoot = ctrlSurfHandles.transform.Find("ctrlHandleTrailingRoot").gameObject;
                    ctrlHandleTrailingTip = ctrlSurfHandles.transform.Find("ctrlHandleTrailingTip").gameObject;
                }
                #endregion

                yield return null;
                yield return null; // unknown how neccesary this is

                Debug.Log("[B9PW] unloading bundle");
                shaderBundle.Unload(false); // unload the raw asset bundle
            }
            else
            {
                Debug.Log("[B9PW] Error: Found no asset bundle to load");
            }
        }
    }
}