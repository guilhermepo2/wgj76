using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransition : MonoBehaviour, ITransitionCamera
{
    public Vector2 transitionDirection;
    void ITransitionCamera.TransitionCamera() {
        CameraScript.instance.TransitCamera(transitionDirection);
    }
}
