using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    private InputActions _inputActions;
    // Start is called before the first frame update
    void Start()
    {
        _inputActions = new InputActions();

        foreach (var action in _inputActions.Game.Get().actions)
        {
            action.performed += ResolveKeyPressed;
            // action.canceled += ResolveKeyPressed; 
        }
    }
    
    private void ResolveKeyPressed(InputAction.CallbackContext context)
    {
        

        string actionName = context.action.name; // This returns the good thing
            
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
