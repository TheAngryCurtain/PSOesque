using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    [SerializeField] private Enums.eCrateType m_Type;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Open();
        }
    }

    private void Open()
    {
        // TODO play some sort of particle fanfare!

        VSEventManager.Instance.TriggerEvent(new GameEvents.RequestItemSpawnEvent(transform.position, Enums.eItemSource.Crate, (Enums.eEnemyType)(-1), m_Type));

        // TODO once there are particles in here, put their duration/playtime as the delay

        Destroy(this.gameObject);
        //StartCoroutine(DestroyAfterDelay(1f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(this.gameObject);
    }
}
