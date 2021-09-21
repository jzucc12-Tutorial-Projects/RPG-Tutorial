using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Control
{
    [RequireComponent(typeof(Pickup))]
    public class ClickablePickUp : MonoBehaviour, IRaycastable
    {
        Pickup pickup;

        private void Awake()
        {
            pickup = GetComponent<Pickup>();
        }

        public CursorType GetCursorType()
        {
            if (pickup.CanBePickedUp())
                return CursorType.pickup;
            else
                return CursorType.fullPickup;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButton(0))
                pickup.PickupItem();

            return true;
        }
    }
}
