using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRotateObject : MonoBehaviour
{
    public float mRotateSpeed = 1f;
    public RectTransform mTransform;
    public bool Reverse = false;

    private void Update()
    {
        if (mTransform != null)
        {
            mTransform.Rotate(0f, 0f, mRotateSpeed * Time.deltaTime * (Reverse ? -1f : 1f));
        }
    }
}
