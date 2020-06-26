using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WingProcedural
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class StaticWingGlobals : MonoBehaviour
    {
        public static List<WingTankConfiguration> wingTankConfigurations = new List<WingTankConfiguration>();

        public static Shader wingShader;

        public static GameObject handlesRoot, handleLength,
                           handleWidthRootFront, handleWidthRootBack, handleWidthTipFront, handleWidthTipBack,
                           handleLeadingRoot, handleLeadingTip, handleTrailingRoot, handleTrailingTip;

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

        private void Awake()
        {
            _bundlePath = KSPUtil.ApplicationRootPath + "GameData" +
                                                    Path.DirectorySeparatorChar +
                                                    "B9_Aerospace_ProceduralWings" + Path.DirectorySeparatorChar + "AssetBundles";
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
                        Debug.Log($"[B9 PWings] Wing shader \"{wingShader.name}\" loaded. Shader supported? {wingShader.isSupported}");
                    }
                }

                #region Handle gizmos---CarnationRED 2020-6
                GameObject[] objects1 = shaderBundle.LoadAllAssets<GameObject>();
                for (int i = 0; i < objects1.Length; i++)
                {
                    GameObject item = objects1[i];
                    if (item.name.Equals("handles"))
                    {
                        handlesRoot = Instantiate(item);
                        break;
                    }
                }
                if (handlesRoot)
                {
                    handlesRoot.SetActive(false);
                    DontDestroyOnLoad(handlesRoot);
                    handleLength = handlesRoot.transform.Find("handle_length").gameObject;

                    handleLength.AddComponent<EditorHandle>();

                    var handleLeading = handlesRoot.transform.Find("handle_leading").gameObject;
                    var handleWidth = handlesRoot.transform.Find("handle_width").gameObject;
                    handleLeading.AddComponent<EditorHandle>();
                    handleWidth.AddComponent<EditorHandle>();

                    handleLeadingRoot = Instantiate(handleLeading);
                    handleLeadingTip = Instantiate(handleLeading);
                    handleTrailingRoot = Instantiate(handleLeading);
                    handleTrailingTip = handleLeading;

                    handleWidthRootFront = Instantiate(handleWidth);
                    handleWidthRootBack = Instantiate(handleWidth);
                    handleWidthTipFront = Instantiate(handleWidth);
                    handleWidthTipBack = handleWidth;

                    handleLeadingRoot.transform.SetParent(handlesRoot.transform);
                    handleLeadingTip.transform.SetParent(handlesRoot.transform);
                    handleTrailingRoot.transform.SetParent(handlesRoot.transform);
                    handleTrailingTip.transform.SetParent(handlesRoot.transform);
                    handleWidthRootFront.transform.SetParent(handlesRoot.transform);
                    handleWidthRootBack.transform.SetParent(handlesRoot.transform);
                    handleWidthTipFront.transform.SetParent(handlesRoot.transform);
                    handleWidthTipBack.transform.SetParent(handlesRoot.transform);

                    handleLength.name = "handleLength";
                    handleLeadingRoot.name = "handleLeadingRoot";
                    handleLeadingTip.name = "handleLeadingTip";
                    handleTrailingRoot.name = "handleTrailingRoot";
                    handleTrailingTip.name = "handleTrailingTip";
                    handleWidthRootFront.name = "handleWidthRootFront";
                    handleWidthRootBack.name = "handleWidthRootBack";
                    handleWidthTipFront.name = "handleWidthTipFront";
                    handleWidthTipBack.name = "handleWidthTipBack";
                    handleLength.layer = 2;
                    handleLeadingRoot.layer = 2;
                    handleLeadingTip.layer = 2;
                    handleTrailingRoot.layer = 2;
                    handleTrailingTip.layer = 2;
                    handleWidthRootFront.layer = 2;
                    handleWidthRootBack.layer = 2;
                    handleWidthTipFront.layer = 2;
                    handleWidthTipBack.layer = 2;
                    handleWidthRootFront.transform.eulerAngles = new Vector3(0, 180, 0);
                    handleWidthRootBack.transform.eulerAngles = new Vector3(0, 180, 0);
                    handleTrailingRoot.transform.eulerAngles = new Vector3(180, 0, 0);
                    handleTrailingTip.transform.eulerAngles = new Vector3(180, 0, 0);
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