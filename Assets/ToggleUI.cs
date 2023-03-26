using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleUI : MonoBehaviour
{
    public InputActionReference inputAction;

    public GameObject ui;

    // Subscribe input-action to Toggle-method.
    private void Awake() {
        inputAction.action.started += Toggle;
    }

    // Start is called before the first frame update
    private void OnDestroy() {
        inputAction.action.started -= Toggle;
    }

    private void Toggle(InputAction.CallbackContext context){
        bool isActive = !ui.activeSelf;
        ui.SetActive(isActive);
    }
}
