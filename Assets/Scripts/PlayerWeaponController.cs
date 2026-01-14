using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponController : MonoBehaviour
{

    public Weapon weapon;

    void Update()
    {
        if (weapon == null)
        {
            Debug.LogWarning("weapon is null");
            return;
        }

#if ENABLE_INPUT_SYSTEM
    if (Mouse.current == null)
    {
        Debug.LogWarning("Mouse.current is null (Active Input Handling を確認)");
        return;
    }

    if (Mouse.current.leftButton.wasPressedThisFrame)
    {
        Debug.Log("CLICK detected");
        var cam = Camera.main;
        Debug.Log("Camera.main = " + (cam ? cam.name : "null"));
        Vector3 world = cam.ScreenToWorldPoint(Mouse.current.position.value);
        weapon.TryFire(world);
    }
#else
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("CLICK detected (legacy)");
            var cam = Camera.main;
            Vector3 world = cam.ScreenToWorldPoint(Input.mousePosition);
            weapon.TryFire(world);
        }
#endif
    }
}