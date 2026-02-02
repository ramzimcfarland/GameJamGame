using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraTarget;
    public float followOffsetX;
    public float followOffsetY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        FollowCameraTargetHorizontally();
    }
    private void FollowCameraTargetHorizontally()
    {
        Vector3 targetPosition = transform.position;
        targetPosition.x = cameraTarget.position.x + followOffsetX;
        targetPosition.y = cameraTarget.position.y + followOffsetY;
        transform.position = targetPosition;
    }
}