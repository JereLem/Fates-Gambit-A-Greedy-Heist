using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public class SetCanvasRenderCamera : MonoBehaviour
    {
        void Awake()
        {
            Canvas canvas = GetComponent<Canvas>();

            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                canvas.worldCamera = Camera.main;
            }
        }
    }
}
