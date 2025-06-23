using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    [Header("스크롤 속도 및 타일 설정")]
    public float scrollSpeed = 1f;
    public float tileWidth = 20f;

    void Update()
    {
        // 왼쪽으로 이동
        transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        // 왼쪽 끝까지 가면 위치 재배치
        if (transform.position.x < -tileWidth)
        {
            transform.position += new Vector3(tileWidth * 2f, 0f, 0f);
        }
    }
}
