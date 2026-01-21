using UnityEngine;

// 대상을 따라다니는 카메라
public class FollowCamera : MonoBehaviour
{
    public Camera m_camera = null; // 카메라

    [SerializeField]
    Transform target = null; // 대상
    public bool isTargetHold = false; // 대상을 변경하지 않는다

    [SerializeField]
    public bool smooth = true; // 부드려운 이동
    public float smoothSpeed = 0.1f; // 부드러운 이동 속도

    public float size = 15;
    public float zoomInSize = 5;
    public float zoomOutSize = 25;
    public float zoomSpeed = 0.2f; // 줌 속도
    bool zoomIn = false;
    bool zoomOut = false;

    private float orthographicSize;

    public Vector3 FixDistance = Vector3.zero; // 카메라와 대상간의 보장된 거리

    public bool cameraFixX = false;
    public bool cameraFixY = false;
    public bool cameraFixZ = false;

    /**/
    public Vector3 shakePos = Vector3.zero;
    public Vector3 localFollowPos = Vector3.zero;

    Vector3 velocity = Vector3.zero;
    Vector3 destination = Vector3.zero;

    public bool IsFollow = false;

    /*
     * *
     */

    private void Awake()
    {
        m_camera = Camera.main;
        localFollowPos = m_camera.transform.position;
        orthographicSize = size;
        m_camera.orthographicSize = orthographicSize;
    }

    public void SetTarget(Transform _target)
    {
        if (isTargetHold == true)
            return;
        target = _target;
    }

    void LateUpdate()
    {
        LookAt();
    }

    private void LookAt()
    {
        if (target == null)
            return;

        Vector3 delta = target.position - localFollowPos;
        destination = localFollowPos + delta;

        if (cameraFixX == true)
            destination.x = localFollowPos.x;
        if (cameraFixY == true)
            destination.y = localFollowPos.y;
        if (cameraFixZ == true)
            destination.z = localFollowPos.z;

        IsFollow = Vector3.Distance(transform.position, destination) < 0.1f;

        localFollowPos = Vector3.SmoothDamp(localFollowPos, destination, ref velocity, smoothSpeed);// * Time.deltaTime);
        transform.position = localFollowPos + FixDistance;

        SetZoom();
    }
   
    public void LookAtNow()
    {
        if (target == null)
            return;

        destination = target.position;

        if (cameraFixX)
            destination.x = localFollowPos.x;
        if (cameraFixY)
            destination.y = localFollowPos.y;
        if (cameraFixZ)
            destination.z = localFollowPos.z;

        localFollowPos = destination;
        transform.position = localFollowPos + FixDistance;
    }

    private void SetZoom()
    {
        if(zoomIn)
        {
            if (orthographicSize > zoomInSize)
                orthographicSize -= zoomSpeed;
            else
                orthographicSize = zoomInSize;
        }
        else if(zoomOut)
        {
            if (orthographicSize < zoomOutSize)
                orthographicSize += zoomSpeed;
            else
                orthographicSize = zoomOutSize;
        }
        else
        {
            float diff = Mathf.Abs(orthographicSize - size);
            if (orthographicSize != size && diff > 0.1f)
            {
                float x = orthographicSize > size ? -1 : 1;
                orthographicSize += zoomSpeed * x;
            }
            else
                orthographicSize = size;
        }
        m_camera.orthographicSize = orthographicSize;
    }

    public void SetZoomIn()
    {
        zoomIn = true;
    }

    public void SetZoomOut()
    {
        zoomOut = true;
    }

    public void ResetZoom()
    {
        zoomIn = false;
        zoomOut = false;
    }

    public void SetSmooth(bool smooth)
    {
        this.smooth = smooth;
    }

    public void LookAtNow(bool _hold)
    {
        isTargetHold = _hold;

        if (_hold == false)
            return;

        LookAtNow();
    }
    
    float GetDistance(Vector3 _target)
    {
        if (_target == null)
            return 0f;

        Vector3 target = _target;
        if (cameraFixX)
            target.x = transform.position.x;
        if (cameraFixY)
            target.y = transform.position.y;
        if (cameraFixZ)
            target.z = transform.position.z;
        
        return Vector2.Distance(target, transform.position);
    }

    public void SetTargetSpeed(float value)
    {
        smoothSpeed = value;
    }
}
