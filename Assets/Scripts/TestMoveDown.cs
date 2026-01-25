using UnityEngine;

public class TestMoveDown : MonoBehaviour
{
    void Update()
    {
        transform.position += Vector3.down * 2f * Time.deltaTime;
    }
}
