using UnityEngine;

[RequireComponent(typeof(Collider))]
public class QTETrigger : MonoBehaviour
{
    public float timeLimit = 1.2f;
    public GameObject hazard;

    bool used;

    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (used) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        used = true;
        QTEManager.Instance.StartQTE(this, player);
    }

    public void Resolve(bool success, PlayerController player)
    {
        if (!success) return;

        player.PlayFlip();
        if (hazard != null) hazard.SetActive(false);
    }
}