using UnityEngine;

public class BounceMarkerController : MonoBehaviour
{
    public float moveSpeed = 8f;

    [Header("Bounds")]
    public float minX = -1.2f;
    public float maxX = 1.2f;
    public float minZ = -2f;
    public float maxZ = 7f;

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(h, 0, v);
        transform.position += move.normalized * moveSpeed * Time.deltaTime;

        ClampPosition();
    }

    void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        transform.position = pos;
    }
}
