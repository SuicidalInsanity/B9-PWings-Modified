/*
    This plugin is attached to a handle object
    Initially added by CarnationRED
*/
using System;
using UnityEngine;

namespace WingProcedural
{
    [DefaultExecutionOrder(65535)]
    public class EditorHandle : MonoBehaviour
    {

        public static EditorHandle draggingHandle;
        public static bool AnyHandleDragging => draggingHandle && draggingHandle.dragging;

        public static float Gain = 0.01f;

        /// <summary>
        /// 0:None, 1:X, 2:Y
        /// </summary>
        private int axisLockState;
        /// <summary>
        /// far: quicker, near: slower
        /// </summary>
        private float cameraDistanceGain = 1f;
        /// <summary>
        /// Incremental values
        /// </summary>
        public float axisX, axisY;

        void Update()
        {
            if (!dragging)
                return;
            if (Input.GetMouseButtonUp(0))
            {
                dragging = false;
                axisX = axisY = 0;
                return;
            }
            switch (axisLockState)
            {
                case 0:
                    if (Input.GetKeyDown(KeyCode.X)) axisLockState = 1;
                    else if (Input.GetKeyDown(KeyCode.Y)) axisLockState = 2;
                    break;
                case 1:
                    if (Input.GetKeyDown(KeyCode.X)) axisLockState = 0;
                    else if (Input.GetKeyDown(KeyCode.Y)) axisLockState = 2;
                    break;
                case 2:
                    if (Input.GetKeyDown(KeyCode.X)) axisLockState = 1;
                    else if (Input.GetKeyDown(KeyCode.Y)) axisLockState = 0;
                    break;
                default:
                    break;
            }

            var offset = Input.mousePosition - mouseLastPos;
            var speedMult = Mathf.Clamp(offset.sqrMagnitude * .025f, 0.4f, 1.25f);
            axisX = axisLockState == 2 ? 0 : (Gain * speedMult * cameraDistanceGain * Vector3.Dot(offset, axisXProjectedToScreen));
            axisY = axisLockState == 1 ? 0 : (Gain * speedMult * cameraDistanceGain * Vector3.Dot(offset, axisYProjectedToScreen));

            if ((axisLockState == 0 && Input.GetKey(KeyCode.LeftControl)) || Input.GetKey(KeyCode.LeftCommand))
            {
                if (axisX * axisY != 0)
                    axisLockState = Math.Abs(axisY) > Math.Abs(axisX) ? 2 : 1;
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.LeftCommand))
                axisLockState = 0;

            mouseLastPos = Input.mousePosition;
        }
        public bool dragging = false;
        private Vector3 mouseLastPos;
        private Vector3 axisXProjectedToScreen;
        private Vector3 axisYProjectedToScreen;

        public void OnMouseOver()
        {
            //if (Input.GetMouseButtonDown(0))
            {
                dragging = true;
                draggingHandle = this;
                mouseLastPos = Input.mousePosition;
                axisXProjectedToScreen = Camera.main.worldToCameraMatrix.MultiplyVector(transform.right).normalized;
                axisYProjectedToScreen = Camera.main.worldToCameraMatrix.MultiplyVector(transform.up).normalized;
                cameraDistanceGain = Mathf.Clamp(Mathf.Abs(Camera.main.worldToCameraMatrix.MultiplyPoint3x4(transform.position).z) * .2f, 0.0625f, 2f);
                axisLockState = 0;

                // Debug.Log($"Xprj:{axisXProjectedToScreen}\tYprj:{axisYProjectedToScreen}");
            }
        }
    }
}