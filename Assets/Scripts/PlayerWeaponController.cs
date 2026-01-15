using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponController : MonoBehaviour
{

    public Weapon weapon;

    // 更新処理
    void Update()
    {

        // マウス左クリックで発射
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            var cam = Camera.main;
            Vector3 world = cam.ScreenToWorldPoint(Mouse.current.position.value);
            weapon.TryFire(world);
        }
    }
}