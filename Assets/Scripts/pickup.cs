using UnityEngine;

public class pickups : MonoBehaviour
{
    [SerializeField] gunStats gun;

    private void OnTriggerEnter(Collider other)
    {
        IPickup pickupable = other.GetComponent<IPickup>();

        if (pickupable != null)
        {
            gun.ammoCur = gun.ammoMax;
            pickupable.getGunStats(gun);
            Destroy(gameObject);
        }

    }


}
