using UnityEngine;

public class AnchorController : MonoBehaviour
{
    public static AnchorController Instance;
    [SerializeField] float rotationSpeed = 100f;
    bool isRotating = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRotating)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    public void Stop()
    {
        isRotating = false;
    }
}
