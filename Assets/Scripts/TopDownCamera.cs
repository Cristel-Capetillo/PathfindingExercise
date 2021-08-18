using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    Transform unit;
    [SerializeField] float distanceFromUnit = 40f;
    [SerializeField] float desiredAngle = 45f;
    [SerializeField] float desiredHeight = 25f;
    [SerializeField] float desiredSmoothness = 0.5f;
    Vector3 referenceToVelocity;

    void Start()
    {
        unit = GameObject.FindGameObjectWithTag("Player").transform;
        ExecuteTopDownCam();
    }

    void LateUpdate()
    {
        ExecuteTopDownCam();
    }
    
    void ExecuteTopDownCam()
    {
        if (!unit)
        {
            return; 
        }
        
        Vector3 worldPosition = (Vector3.forward * -distanceFromUnit) +(Vector3.up * desiredHeight);
        Vector3 angleToViewFrom = Quaternion.AngleAxis(desiredAngle, Vector3.up) * worldPosition;
        Vector3 flatPosition = unit.position;
        flatPosition.y = 0;
        Vector3 finalPosition = flatPosition + angleToViewFrom;
        transform.position =
            Vector3.SmoothDamp(transform.position, finalPosition, ref referenceToVelocity, desiredSmoothness);
        transform.LookAt(flatPosition);
    }
}