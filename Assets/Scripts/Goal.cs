using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Jeff jeff = other.GetComponent<Jeff>();

        if (jeff != null )
        {
            SceneManager.LoadScene("WIN");
        }
    }
}
