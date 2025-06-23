using UnityEngine;

public class HomeCamera : MonoBehaviour
{
    [Header("카메라 설정")]
    public Transform target;

    private void Update()
    {
        if (target != null)
        {
            // Y축만 따라가고, X와 Z는 고정
            transform.position = new Vector3(transform.position.x, target.position.y, transform.position.z);
        }
    }
}
