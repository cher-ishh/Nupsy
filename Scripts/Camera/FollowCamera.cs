using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("카메라 설정")]
    public Transform target;
    public float zoom = 3.5f;

    private void Start()
    {
        Camera.main.orthographicSize = zoom;
    }

    private void Update()
    {
        if (target != null)
        {
            // 대상 위치를 따라가되, Z축은 고정
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }
    }
}
