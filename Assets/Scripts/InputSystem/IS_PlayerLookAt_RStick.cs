using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IS_PlayerLookAt_RStick : MonoBehaviour
{
    private Vector2 axis;
    private void Update()
    {
       
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        // LookActionの入力値を取得
        axis = context.ReadValue<Vector2>().normalized;
    }

    public Vector2 GetLookAxis() => axis;

}
