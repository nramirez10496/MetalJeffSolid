using UnityEngine;
using UnityEngine.Events;

public class Sensor : MonoBehaviour
{
    [SerializeField] UnityEvent<Collider> OnHeard;

    private void OnTriggerStay(Collider other)
    {
        OnHeard.Invoke(other);
    }
}
