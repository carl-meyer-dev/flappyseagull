using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed = 1f;

    // Update is called once per frame
    private void Update()
    {
        transform.position += Vector3.right * (speed * Time.deltaTime);
    }
}