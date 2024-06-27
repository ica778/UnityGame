using FishNet.Connection;
using FishNet.Demo.AdditiveScenes;
using FishNet.Object;
using UnityEngine;

public class PlayerItemHolder : NetworkBehaviour {
    [SerializeField] private Transform rightHand;
    [SerializeField] private Animator rightHandAnimator;

    private ItemSO currentItem;
    private GameObject currentItemObject;

    private WeaponCollisionDetector wcd;

    // Animations
    private const string TRIGGER_ATTACK = "Attack";

    private void InventoryManager_OnSelectedItemChanged(object sender, InventoryManager.OnSelectedItemChangedEventArgs e) {
        if (currentItemObject) {
            Destroy(currentItemObject);
        }

        if (e.itemId == -1) {
            currentItem = null;
            currentItemObject = null;
        }
        else {
            currentItem = ItemDatabase.Instance.GetItem(e.itemId);
            currentItemObject = Instantiate(currentItem.GetEquippedObject(), rightHand);
            switch (currentItem.GetItemType()) {
                case (ItemType.Weapon):
                    wcd = GetComponentInChildren<WeaponCollisionDetector>();
                    break;
            }
        }
    }

    private void GameInput_OnLeftClickStartedAction(object sender, System.EventArgs e) {
        if (!currentItem) {
            return;
        }

        switch (currentItem.GetItemType()) {
            case (ItemType.Weapon):
                AttackWithWeapon();
                break;
        }
    }

    private void AttackWithWeapon() {
        rightHandAnimator.SetTrigger(TRIGGER_ATTACK);
    }

    private void OnEnable() {
        InventoryManager.Instance.OnSelectedItemChanged += InventoryManager_OnSelectedItemChanged;
        GameInput.Instance.OnLeftClickStartedAction += GameInput_OnLeftClickStartedAction;
    }

    private void OnDisable() {
        InventoryManager.Instance.OnSelectedItemChanged -= InventoryManager_OnSelectedItemChanged;
        GameInput.Instance.OnLeftClickStartedAction -= GameInput_OnLeftClickStartedAction;
    }
}