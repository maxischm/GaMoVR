/**
 * Copyright (c) 2017 The Campfire Union Inc - All Rights Reserved.
 *
 * Licensed under the MIT license. See LICENSE file in the project root for
 * full license information.
 *
 * Email:   info@campfireunion.com
 * Website: https://www.campfireunion.com
 *
 * Edited by Simon C. Gorissen (scg@mail.upb.de)
 */

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VRKeys;

[RequireComponent(typeof(EventSystem))]
public class KeyboardManager : MonoBehaviour
{
    public Vector3 standardKeyboardPos = new Vector3();

    public GameObject keyboardPrefab;

    /// <summary>
    /// Reference to the VRKeys keyboard.
    /// </summary>
    private Keyboard keyboard;

    private EventSystem eventSystem;

    private GameObject oldSelectedGO;

    void Start()
    {
        this.eventSystem = GetComponent<EventSystem>();
        GameObject keyboardGO = Instantiate(keyboardPrefab);
        keyboard = keyboardGO.GetComponent<Keyboard>();

        keyboardGO.transform.position = standardKeyboardPos;

        //keyboard.Enable();
        oldSelectedGO = eventSystem.currentSelectedGameObject;

        keyboard.OnSubmit.AddListener(HandleSubmit);
        keyboard.OnCancel.AddListener(HandleCancel);

        keyboard.Disable();
    }

    private void OnDisable()
    {
        keyboard.OnSubmit.RemoveListener(HandleSubmit);
        keyboard.OnCancel.RemoveListener(HandleCancel);

        keyboard.Disable();
    }

    public void Update()
    {
#if UNITY_EDITOR
        /*if(Input.GetKeyDown(KeyCode.Space))
		{
			keyboard.Enable();
		}*/
#endif

        if (oldSelectedGO != eventSystem.currentSelectedGameObject)
        {
            //New element selected
            oldSelectedGO = eventSystem.currentSelectedGameObject;
            if (oldSelectedGO != null)
            {
                InputField selectedInputField = oldSelectedGO.GetComponent<InputField>();
                if (selectedInputField != null)
                {
                    keyboard.Enable();
                    keyboard.activeInputField = selectedInputField;
                    return;
                }
            }
            keyboard.Disable();
            keyboard.activeInputField = null;
        }
    }

    public void HandleSubmit(string text)
    {
        keyboard.Disable();
        oldSelectedGO = null;
        eventSystem.SetSelectedGameObject(null);
    }

    public void HandleCancel()
    {
        keyboard.Disable();
        oldSelectedGO = null;
        eventSystem.SetSelectedGameObject(null);
    }
}
