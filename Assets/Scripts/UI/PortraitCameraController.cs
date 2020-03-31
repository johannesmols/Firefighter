using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitCameraController : MonoBehaviour
{
    private Transform cameraTransform;
    public Transform followTransform;

    private void Awake() {
        cameraTransform = transform.Find("PortraitCamera");
        Show(this.followTransform);
    }

    private void Update() {
        cameraTransform.position = new Vector3(followTransform.position.x, followTransform.position.y+1, followTransform.position.z-1);
    }

    public void Show(Transform followTransform) {
        gameObject.SetActive(true);
        this.followTransform = followTransform;
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
