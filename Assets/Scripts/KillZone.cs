using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KillZone : MonoBehaviour
{
    public float respawnForward = 8f;

    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc == null) return;

        LivesManager.Instance.LoseLife();

        Vector3 p = other.transform.position;
        cc.enabled = false;
        other.transform.position = new Vector3(p.x, 1f, p.z + respawnForward);
        cc.enabled = true;
    }
}