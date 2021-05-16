using System;

namespace WingProcedural
{
    public class WPDebug : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Log Settings"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "B9 Procedural Wings"; } }
        public override string DisplaySection { get { return "B9 Procedural Wings"; } }
        public override int SectionOrder { get { return 2; } }
        public override bool HasPresets => false;


        [GameParameters.CustomParameterUI("Enable Aero Logging")]
        public bool logCAV = false;

        [GameParameters.CustomParameterUI("Enable Update Logging")]
        public bool logUpdate = false;

        [GameParameters.CustomParameterUI("Enable Geometry Logging")]
        public bool logUpdateGeometry = false;

        [GameParameters.CustomParameterUI("Enable Material Logging")]
        public bool logUpdateMaterials = false;

        [GameParameters.CustomParameterUI("Enable Mesh ref Logging")]
        public bool logMeshReferences = false;

        [GameParameters.CustomParameterUI("Enable Check Mesh Logging")]
        public bool logCheckMeshFilter = false;

        [GameParameters.CustomParameterUI("Enable Property Logging")]
        public bool logPropertyWindow = false;

        [GameParameters.CustomParameterUI("Enable Flight Setup Logging")]
        public bool logFlightSetup = false;

        [GameParameters.CustomParameterUI("Enable Field Setup Logging")]
        public bool logFieldSetup = false;

        [GameParameters.CustomParameterUI("Enable Fuel Logging")]
        public bool logFuel = false;

        [GameParameters.CustomParameterUI("Enable Limits Logging")]
        public bool logLimits = false;

        [GameParameters.CustomParameterUI("Enable Events Logging")]
        public bool logEvents = false;

    }

    public class WPActivation : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Activation"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "B9 Procedural Wings"; } }
        public override string DisplaySection { get { return "B9 Procedural Wings"; } }

        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets => false;

        [GameParameters.CustomParameterUI("Use old keycode to activate")]
        public bool useKeycodeToActivate = false;
        [GameParameters.CustomFloatParameterUI("Hover timeout",
            minValue = 0.5f, maxValue = 5.0f, stepCount = 46, displayFormat = "F1",
            toolTip = "Time to wait after moving mouse off part before disabling edit mode"
            )]
        public double hoverEditTimeout = 1.0f;


    }
}