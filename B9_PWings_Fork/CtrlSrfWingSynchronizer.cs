#if FAR
//using ferram4;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WingProcedural
{
    /// <summary>
    /// This is to make connected control surface wings deflects like one big wing
    /// </summary>
    [DefaultExecutionOrder(25565)]
    public class CtrlSrfWingSynchronizer : MonoBehaviour
    {
        public List<WingProcedural> connectedCtrlSrfWings;
        public List<ModuleControlSurface> ctrlSrfs;
        public List<Quaternion> ctrlSrfsNeutral;
        public List<Transform> ctrlSrfTrans;

        public float deflectionLastFrame;

        private static readonly FieldInfo deflection;
        private static readonly FieldInfo neutral;
        private static readonly FieldInfo ctrlSurface;
        static CtrlSrfWingSynchronizer()
        {
            deflection = typeof(ModuleControlSurface).GetField("deflection", (BindingFlags)0b1111111111111111111111111);
            neutral = typeof(ModuleControlSurface).GetField("neutral", (BindingFlags)0b1111111111111111111111111);
            ctrlSurface = typeof(ModuleControlSurface).GetField("ctrlSurface", (BindingFlags)0b1111111111111111111111111);
        }
        public void Start()
        {
            ctrlSrfs = connectedCtrlSrfWings.Select(wp => wp.part.FindModuleImplementing<ModuleControlSurface>()).ToList();
            ctrlSrfsNeutral = ctrlSrfs.Select(cs => (Quaternion)neutral.GetValue(cs)).ToList();
            ctrlSrfTrans = ctrlSrfs.Select(cs => (Transform)ctrlSurface.GetValue(cs)).ToList();
        }

        static internal Type FARType = null;
        static public void InitFAR()
        {
            if (FARType != null) return;
            Debug.Log("LGG InitFAR 1");
            foreach (AssemblyLoader.LoadedAssembly test in AssemblyLoader.loadedAssemblies)
            {
                if (test.assembly.GetName().Name.Equals("FerramAerospaceResearch", StringComparison.InvariantCultureIgnoreCase))
                {
                    // This seems to work
                    FARType = test.assembly.GetType("ferram4.FARControllableSurface");
                    Debug.Log("LGG InitFAR, type: " + FARType.ToString());
#if false
                    // This retuns null
                    object asmInstance = Activator.CreateInstance(FARType);
                    Debug.Log("InitFAR, asmInstance: " + asmInstance.ToString());

                    // This also returns null
                    FARControllableSurface fcs = new FARControllableSurface();
                    Debug.Log("InitFAR, fcs: " + fcs.ToString());

                    if (fcs == null)
                        Debug.Log("InitFAR, fcs is null");
                    else
                        Debug.Log("InitFAR, fcs: " + fcs.ToString());
#endif
                    break;
                }
            }
            

        }

        public void FixedUpdate()
        {
            var deflectionAngle = 0f;
            float maxDelta = 0f;
            for (int i = 0; i < connectedCtrlSrfWings.Count; i++)
            {
                var c = connectedCtrlSrfWings[i];
                if (!c || !c.part)
                {
                    connectedCtrlSrfWings.RemoveAt(i);
                    ctrlSrfs.RemoveAt(i);
                    ctrlSrfsNeutral.RemoveAt(i);
                    ctrlSrfTrans.RemoveAt(i--);
                    if (connectedCtrlSrfWings.Count == 1)
                        Destroy(this);
                    continue;
                }

                //Get max deflection
                var cs = ctrlSrfs[i];
                var d = (float)deflection.GetValue(cs);
                if (Mathf.Abs(d) > deflectionAngle)
                    deflectionAngle = d;

                maxDelta = Mathf.Max((Time.fixedDeltaTime * cs.actuatorSpeed), maxDelta);
            }

            //Fix a bug when the wing is near vessel's centerline
            float delta = (float)(deflectionAngle - deflectionLastFrame);
            if (Mathf.Abs(delta) > (maxDelta + 0.1f))
                deflectionAngle = deflectionLastFrame + maxDelta * Mathf.Sign(delta);
            deflectionLastFrame = deflectionAngle;

            //apply deflection
            for (int i = 0; i < ctrlSrfTrans.Count; i++)
                ctrlSrfTrans[i].localRotation = Quaternion.AngleAxis(deflectionAngle, Vector3.right) * ctrlSrfsNeutral[i];
        }

        public static void AddSynchronizer(Part p, List<WingProcedural> connectedCtrlSrfWings)
        {
            p.gameObject.AddComponent<CtrlSrfWingSynchronizer>().connectedCtrlSrfWings = connectedCtrlSrfWings;
        }
#if FAR
        public static void FARAddSynchronizer(Part p, List<WingProcedural> connectedCtrlSrfWings)
        {
            p.gameObject.AddComponent<FARCtrlSrfWingSynchronizer>().connectedCtrlSrfWings = connectedCtrlSrfWings;
        }
#endif
    }

#if FAR
    /// <summary>
    /// This is to make connected control surface wings deflects like one big wing
    /// </summary>
    [DefaultExecutionOrder(25565)]
    public class FARCtrlSrfWingSynchronizer : MonoBehaviour
    {
        public List<WingProcedural> connectedCtrlSrfWings;
#if false
        public List<FARControllableSurface> farCtrlSrfs;
#else
        public List<object> farCtrlSrfs;
#endif
        public double deflectionLastFrame;

        private static readonly FieldInfo AoAoffset;
        private static readonly MethodInfo DeflectionAnimation;
        static FARCtrlSrfWingSynchronizer()
        {
#if false
            AoAoffset = typeof(FARControllableSurface).GetField("AoAoffset", (BindingFlags)0b1111111111111111111111111);
            DeflectionAnimation = typeof(FARControllableSurface).GetMethod("DeflectionAnimation", (BindingFlags)0b1111111111111111111111111);
#else
            AoAoffset = CtrlSrfWingSynchronizer.FARType.GetField("AoAoffset", (BindingFlags)0b1111111111111111111111111);
            DeflectionAnimation = CtrlSrfWingSynchronizer.FARType.GetMethod("DeflectionAnimation", (BindingFlags)0b1111111111111111111111111);
#endif
        }

#if false
        public void Start() => farCtrlSrfs = connectedCtrlSrfWings.Select(wp => wp.part.FindModuleImplementing<FARControllableSurface>()).ToList();
#else
        public void Start()
        {
            farCtrlSrfs = new List<object>();
            foreach (var csw in connectedCtrlSrfWings)
            {
                foreach (var m in csw.part.Modules)
                {
                    if (m.moduleName == "FARControllableSurface")
                    {
                        farCtrlSrfs.Add((object)m);
                    }
                }
            }
        }
            
//            => farCtrlSrfs = connectedCtrlSrfWings.Select(wp => wp.part.FindModuleImplementing(FARType)()).ToList();

#endif
        public void FixedUpdate()
        {
            var deflectionAngle = 0d;
            float maxDelta = 0f;
            for (int i = 0; i < connectedCtrlSrfWings.Count; i++)
            {
                var c = connectedCtrlSrfWings[i];
                if (!c || !c.part)
                {
                    connectedCtrlSrfWings.RemoveAt(i);
                    farCtrlSrfs.RemoveAt(i--);
                    if (connectedCtrlSrfWings.Count == 1)
                        Destroy(this);
                    continue;
                }

                //Get max deflection
                var cs = farCtrlSrfs[i];
                var d = (double)AoAoffset.GetValue(cs);
                if (Math.Abs(d) > deflectionAngle)
                    deflectionAngle = d;

                ModuleControlSurface mcs = (ModuleControlSurface)cs;
                maxDelta = Mathf.Max((Time.fixedDeltaTime * mcs.actuatorSpeed), maxDelta);
            }

            //Fix a bug when the wing is near vessel's centerline
            float delta = (float)(deflectionAngle - deflectionLastFrame);
            if (Mathf.Abs(delta) > (maxDelta + 0.1f))
                deflectionAngle = deflectionLastFrame + maxDelta * Mathf.Sign(delta);
            deflectionLastFrame = deflectionAngle;

            //apply deflection
            foreach (var farcs in farCtrlSrfs)
            {
                AoAoffset.SetValue(farcs, deflectionAngle);
                DeflectionAnimation.Invoke(farcs, null);
            }
        }
    }
#endif
    }
