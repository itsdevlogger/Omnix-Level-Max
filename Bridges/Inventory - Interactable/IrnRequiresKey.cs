using InteractionSystem;
using UnityEngine;


namespace InventorySystem
{
    public class IrnRequiresKey : MonoBehaviour, IInteractionCriteria
    {
        private const string COMPONENT = nameof(InventoryPlayer);
        
        [SerializeField, Tooltip("If direct referance is possible, use this")] private InventoryItem _key;
        [SerializeField, Tooltip("If direct referance is not possible, use this")] private string _keyId;
        [SerializeField] private string _errorMessage;

        private string KeyID
        {
            get
            {
                if (_key != null) return _key.ID;
                return _keyId;
            }
        }

        private InventoryPlayer _inventoryPlayer;

        bool IInteractionCriteria.CanInteract(InteractionPlayer player)
        {
            if (_inventoryPlayer == null || _inventoryPlayer.gameObject != player.gameObject)
                _inventoryPlayer = player.GetComponent<InventoryPlayer>();
            if (_inventoryPlayer == null)
            {
                Debug.LogError($"Player \"{player.name}\" does not have \"{COMPONENT}\" component");
                InteractionUiHandler.DisplayError($"Player dont have \"{COMPONENT}\" component");
                return false;
            }

            if (!_inventoryPlayer.HasItemInInventory(KeyID))
            {
                InteractionUiHandler.DisplayError(_errorMessage);
                return false;
            }

            return true;
        }
    }
}