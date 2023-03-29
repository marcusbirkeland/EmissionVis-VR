using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleUI : MonoBehaviour
{
    public InputActionReference [] inputActions;

    public GameObject ui;

    // Subscribe each input-action to Toggle-method.
    private void Awake() {
        foreach(InputActionReference iar in inputActions){
            iar.action.started += Toggle;
        }
        Toggle();
    }

    // Start is called before the first frame update
    private void OnDestroy() {
        foreach(InputActionReference iar in inputActions){
            iar.action.started -= Toggle;
        }
    }

    private void Toggle(InputAction.CallbackContext context){
        bool isActive = !ui.activeSelf;
        ui.SetActive(isActive);
    }

    private void Toggle(){
        bool isActive = !ui.activeSelf;
        ui.SetActive(isActive);
    }
}
