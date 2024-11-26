using InteractionSystem;
using UnityEngine;

namespace InventorySystem
{
    [RequireComponent(typeof(Collider))]
    public class IrnItemKey : InventoryItem, IInteractionProcessor
    {
        [SerializeField] private bool _immedietEndInteraction = true;

        protected override void Start()
        {
            base.Start();

            if (IsPicked) 
                gameObject.SetActive(false);
        }

        void IInteractionProcessor.OnInteractionStart(InteractionPlayer player)
        {
            gameObject.SetActive(false);
            var inventory = player.GetComponent<InventoryPlayer>();
            if (inventory == null)
                Debug.LogError($"Cant pickup item as the player does not have \"{nameof(InventoryPlayer)}\" component");
            else
                inventory.PickupItem(this);
            
            if (_immedietEndInteraction) 
                player.EndInteraction();
        }

        void IInteractionProcessor.OnInteractionEnd(InteractionPlayer player)
        {

        }

        protected override void OnDropped(InventoryPlayer player)
        {

        }

        protected override void OnPicked(InventoryPlayer player)
        {

        }

        protected override void OnUse(InventoryPlayer player)
        {

        }
    }
}