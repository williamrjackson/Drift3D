using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchAxisCtrl : MonoBehaviour {
    public bool spawnOnTouch;
    public Transform node;
    public float touchArea = 1.25f;
    public float nodeRange = 1f;

    int m_CapturedTouch = -1;
    Vector2 m_Axis;
    Vector3 m_InitialScale;
    //Camera m_MainCamera;
    //Camera m_JoyCamera;
	
    // Use this for initialization
	void Start () {
        if (node.parent != transform)
        {
            node.parent = transform;
        }
        node.position = transform.position;
        m_InitialScale = transform.localScale;
        Reset();
	}

    void Update () {
        // Get the mouse position - this is for testing. 
        // If not in the unity editor, get the touch position. Either only care about a touch in the right zone, 
        // or use the spawn option and only care about the first touch.
#if (UNITY_IOS || UNITY_ANDROID)
        if (m_CapturedTouch < 0)
        {
            for (int i = 0; i < Input.touchCount; ++i)
            {
                if ((!spawnOnTouch && (Input.GetTouch(i).phase == TouchPhase.Began) && 
                    (GetPointDistance(m_JoyCamera.ScreenToWorldPoint(Input.GetTouch(i).position)) < GetScaledParimeter(touchArea))) 
                    || (spawnOnTouch && Input.GetTouch(i).phase == TouchPhase.Began))
                {
                    CaptureTouch(i, m_JoyCamera.ScreenToWorldPoint(Input.GetTouch(i).position));
                }
            }
        }
        else
        {
            if (Input.GetTouch(m_CapturedTouch).phase == TouchPhase.Ended)
            {
                Reset();
            }
            else
            {
                HandleValidTouch(m_JoyCamera.ScreenToWorldPoint(Input.GetTouch(m_CapturedTouch).position));
            }
        }
#else
        if (m_CapturedTouch < 0)
        {
            if ((!spawnOnTouch && Input.GetMouseButtonDown(0) && GetPointDistance(Input.mousePosition) < GetScaledParimeter(touchArea))
                || (spawnOnTouch && Input.GetMouseButtonDown(0)))
            {
                CaptureTouch(0, Input.mousePosition);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Reset();
        }
        else
        {
            HandleValidTouch(Input.mousePosition);
        }
#endif 
    }

    void HandleValidTouch(Vector2 currentPos)
    {
        if (GetPointDistance(currentPos) > GetScaledParimeter(nodeRange))
        {
            node.position = transform.position + (ToVector3(currentPos) - transform.position).normalized * GetScaledParimeter(nodeRange);
        }
        else
        {
            node.position = currentPos;
        }
        m_Axis = (node.localPosition - Vector3.zero).normalized * Remap(GetPointDistance(currentPos), 0, GetScaledParimeter(touchArea), 0, 1);
    }

    void CaptureTouch(int touchIndex, Vector2 touchPos)
    {
        if (spawnOnTouch)
        {
            transform.position = touchPos;
            transform.localScale = m_InitialScale;
        }
        m_CapturedTouch = touchIndex;
    }
    void Reset()
    {
        if (spawnOnTouch)
        {
            transform.localScale = Vector3.zero;
        }
        node.position = transform.position;
        m_Axis = Vector2.zero;
        m_CapturedTouch = -1;
    }

    float GetPointDistance(Vector2 point)
    {
        return Vector2.Distance(point, transform.position);
    }

    // __ HELPER FUNCTIONS __ //
    Vector3 ToVector3(Vector2 v2)
    {
        return new Vector3(v2.x, v2.y, 0f);
    }
    float GetScaledParimeter(float parimeter)
    {
        return parimeter * (transform.localScale.magnitude / 2);
    }
    float Remap(float val, float srcMin, float srcMax, float destMin, float destMax)
    {
        return Mathf.Lerp(destMin, destMax, Mathf.InverseLerp(srcMin, srcMax, val));
    }

    // __ PUBLIC __ //
    public Vector2 GetAxis()
    {
        return m_Axis;
    }
    public float GetAxis(string vertOrHorizontal)
    {
        if (vertOrHorizontal == "Horizontal")
            return m_Axis.x;
        else if (vertOrHorizontal == "Vertical")
            return m_Axis.y;
        if (vertOrHorizontal == "InverseHorizontal")
            return Remap(m_Axis.x, -1, 1, 1, -1);
        else if (vertOrHorizontal == "InverseVertical")
            return Remap(m_Axis.y, -1, 1, 1, -1);
        else
            return 0f;
    }

    // __ EDITOR GIZMOS __ //
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, GetScaledParimeter(touchArea));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, GetScaledParimeter(nodeRange));
    }
}
