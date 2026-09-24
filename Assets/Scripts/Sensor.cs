using UnityEngine;
using UnityEngine.Events;

public class Sensor : MonoBehaviour
{
    [SerializeField] UnityEvent<Collider> OnHeard;
    [SerializeField] UnityEvent<Collider> OnLeft;

    private void OnTriggerStay(Collider other)
    {
        OnHeard.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnLeft.Invoke(other);
    }
}
