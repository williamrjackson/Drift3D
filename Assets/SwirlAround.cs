using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwirlAround : MonoBehaviour {

    public Vector3 swirlRange = new Vector3( 1, 1, 1 );
    public float swirlTime = 1f;
    public float smoothTime = 0.4f;
    public bool enableFollow = true;
    private GameObject m_FollowMe;
    private Vector3 m_SmoothVelocity = Vector3.zero;
    private Vector3 m_InitialPos;

    // Use this for initialization
    void Start()
    {
        m_InitialPos = transform.localPosition;
        StartCoroutine( Swirl() );
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.localPosition = Vector3.SmoothDamp( transform.localPosition, m_FollowMe.transform.localPosition, ref m_SmoothVelocity, smoothTime );
    }

    public IEnumerator MoveLocal(Transform transformToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = transformToMove.localPosition;
        while (elapsedTime < seconds)
        {
            transformToMove.localPosition = Vector3.Lerp( startingPos, end, (elapsedTime / seconds) );
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transformToMove.localPosition = end;
    }

    private IEnumerator Swirl()
    {
        m_FollowMe = new GameObject();
        m_FollowMe.transform.parent = transform.parent;
        m_FollowMe.transform.localPosition = m_InitialPos;
        while (true)
        {
            if (enableFollow)
            {
                float upperRangeX = Mathf.Abs( swirlRange.x );
                float upperRangeY = Mathf.Abs( swirlRange.y );
                float upperRangeZ = Mathf.Abs( swirlRange.z );
                StartCoroutine( MoveLocal( m_FollowMe.transform, m_InitialPos + new Vector3( Random.Range( -upperRangeX, upperRangeX ), Random.Range( -upperRangeY, upperRangeY ), Random.Range( -upperRangeZ, upperRangeZ ) ), swirlTime ) );
                yield return new WaitForSeconds( swirlTime );
            }
            else
            {
                StartCoroutine( MoveLocal( m_FollowMe.transform, m_InitialPos, swirlTime ) );
                yield return new WaitForSeconds( swirlTime );
            }

        }
    }
}
