﻿using System;
using UnityEngine;

namespace WingProcedural
{
    [Serializable]
    public class WingDefaults : IConfigNode
    {
        [Persistent]
        public Vector4 baseLength = new Vector4(4f, 1f, 4f, 1f);
        [Persistent]
        public Vector4 baseWidthRoot = new Vector4(4f, 0.5f, 4f, 0.5f);
        [Persistent]
        public Vector4 baseWidthTip = new Vector4(4f, 0.5f, 4f, 0.5f);
        [Persistent]
        public Vector4 baseOffsetRoot = new Vector4(0f, 0f, 0f, 0f);
        [Persistent]
        public Vector4 baseOffsetTip = new Vector4(0f, 0f, 0f, 0f);
        [Persistent]
        public Vector4 baseThicknessRoot = new Vector4(0.24f, 0.24f, 0.24f, 0.24f);
        [Persistent]
        public Vector4 baseThicknessTip = new Vector4(0.24f, 0.24f, 0.24f, 0.24f);
        [Persistent]
        public Vector4 edgeTypeLeading = new Vector4(2f, 1f, 2f, 1f);
        [Persistent]
        public Vector4 edgeWidthLeadingRoot = new Vector4(0.24f, 0.24f, 0.24f, 0.24f);
        [Persistent]
        public Vector4 edgeWidthLeadingTip = new Vector4(0.24f, 0.24f, 0.24f, 0.24f);
        [Persistent]
        public Vector4 edgeTypeTrailing = new Vector4(3f, 2f, 3f, 2f);
        [Persistent]
        public Vector4 edgeWidthTrailingRoot = new Vector4(0.48f, 0.48f, 0.48f, 0.48f);
        [Persistent]
        public Vector4 edgeWidthTrailingTip = new Vector4(0.48f, 0.48f, 0.48f, 0.48f);

        public WingDefaults(ConfigNode node)
        {
            Load(node);
        }

        public void Load(ConfigNode node)
        {
            ConfigNode.LoadObjectFromConfig(this, node);
        }

        public void Save(ConfigNode node)
        {
            ConfigNode.CreateConfigFromObject(this, node);
        }

        public void Apply()
        {
            WingProcedural.sharedBaseLengthDefaults = baseLength;
            WingProcedural.sharedBaseWidthRootDefaults = baseWidthRoot;
            WingProcedural.sharedBaseWidthTipDefaults = baseWidthTip;
            WingProcedural.sharedBaseOffsetRootDefaults = baseOffsetRoot;
            WingProcedural.sharedBaseOffsetTipDefaults = baseOffsetTip;
            WingProcedural.sharedBaseThicknessRootDefaults = baseThicknessRoot;
            WingProcedural.sharedBaseThicknessTipDefaults = baseThicknessTip;
            WingProcedural.sharedEdgeTypeLeadingDefaults = edgeTypeLeading;
            WingProcedural.sharedEdgeWidthLeadingRootDefaults = edgeWidthLeadingRoot;
            WingProcedural.sharedEdgeWidthLeadingTipDefaults = edgeWidthLeadingTip;
            WingProcedural.sharedEdgeTypeTrailingDefaults = edgeTypeTrailing;
            WingProcedural.sharedEdgeWidthTrailingRootDefaults = edgeWidthTrailingRoot;
            WingProcedural.sharedEdgeWidthTrailingTipDefaults = edgeWidthTrailingTip;
        }
    }
}