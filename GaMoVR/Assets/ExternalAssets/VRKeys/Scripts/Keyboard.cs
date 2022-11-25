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

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR;

namespace VRKeys
{

    /// <summary>
    /// Keyboard input system for use with NewtonVR. To use, drop the VRKeys prefab
    /// into your scene and activate as needed. Listen for OnUpdate and OnSubmit events,
    /// and set the text via SetText(string).
    ///
    /// Input validation can be done during OnUpdate and OnSubmit events by calling
    /// ShowValidationMessage(msg) and HideValidationMessage(). The keyboard does not
    /// automatically hide OnSubmit, but rather you should call SetActive(false) when
    /// you have finished validating the submitted text.
    /// </summary>
    public class Keyboard : MonoBehaviour
    {

        //public Vector3 positionRelativeToUser = new Vector3 (0f, 1.35f, 2f);

        public KeyboardLayout keyboardLayout = KeyboardLayout.Qwerty;

        [Space(15)]
        public Color displayTextColor = Color.black;

        public Color caretColor = Color.gray;

        [Space(15)]
        public GameObject keyPrefab;

        public Transform keysParent;

        public GameObject keyboardHandle;

        public float keyWidth = 0.16f;

        public float keyHeight = 0.16f;

        [Space(15)]

        public GameObject leftMallet;

        public GameObject rightMallet;

        public GameObject keyboardWrapper;

        public ShiftKey shiftKey;

        public Key[] extraKeys;

        [Space(15)]
        public bool leftPressing = false;

        public bool rightPressing = false;

        public bool initialized = false;

        public bool disabled = true;

        [Serializable]
        public class KeyboardUpdateEvent : UnityEvent<string> { }


        [Serializable]
        public class KeyboardSubmitEvent : UnityEvent<string> { }

        [Space(15)]

        /// <summary>
        /// Listen for events whenever the text changes.
        /// </summary>
        public KeyboardUpdateEvent OnUpdate = new KeyboardUpdateEvent();

        /// <summary>
        /// Listen for events when Submit() is called.
        /// </summary>
        public KeyboardSubmitEvent OnSubmit = new KeyboardSubmitEvent();

        /// <summary>
        /// Listen for events when Cancel() is called.
        /// </summary>
        public UnityEvent OnCancel = new UnityEvent();

        private GameObject player;

        private GameObject leftHand;

        private GameObject rightHand;

        private LetterKey[] keys;

        private bool shifted = false;

        private Layout layout;

        public InputField activeInputField;

        [Space(20)]

        [SerializeField] private UnityEvent enable;
        [SerializeField] private UnityEvent disable;

        /// <summary>
        /// Initialization.
        /// </summary>
        private IEnumerator Start()
        {
            XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale);

            if (keyboardHandle == null)
            {
                keyboardHandle = keysParent.gameObject;//if no handle is assigned, ignore it
            }

            player = GameObject.FindGameObjectWithTag("LocalPlayer");
            Debug.Log($"Keyboard Player: {player}");
            //playerSpace.transform.localPosition = InputTracking.GetLocalPosition (XRNode.TrackingReference);
            //playerSpace.transform.localRotation = InputTracking.GetLocalRotation (XRNode.TrackingReference);

            leftHand = GameObject.FindGameObjectWithTag("OpenXRLeftHand");
            rightHand = GameObject.FindGameObjectWithTag("OpenXRRightHand");

            yield return StartCoroutine(DoSetLanguage(keyboardLayout));

            initialized = true;
        }

        /*
		private void Update () 
		{
			//playerSpace.transform.localPosition = InputTracking.GetLocalPosition (XRNode.TrackingReference);
			//playerSpace.transform.localRotation = InputTracking.GetLocalRotation (XRNode.TrackingReference);
			
			leftHand.transform.localPosition = InputTracking.GetLocalPosition(XRNode.LeftHand);
			leftHand.transform.localRotation = InputTracking.GetLocalRotation(XRNode.LeftHand);

			rightHand.transform.localPosition = InputTracking.GetLocalPosition(XRNode.RightHand);
			rightHand.transform.localRotation = InputTracking.GetLocalRotation(XRNode.RightHand);
			
		}
		*/

        private void PositionAndAttachMallets()
        {
            //transform.SetParent (player.transform, false);
            //transform.localPosition = positionRelativeToUser;

            leftMallet.transform.SetParent(leftHand.transform);
            leftMallet.transform.localPosition = Vector3.zero;
            leftMallet.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            leftMallet.SetActive(true);

            rightMallet.transform.SetParent(rightHand.transform);
            rightMallet.transform.localPosition = Vector3.zero;
            rightMallet.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            rightMallet.SetActive(true);
        }

        private void DetachMallets()
        {
            leftMallet?.SetActive(false);

            rightMallet?.SetActive(false);
        }

        /// <summary>
        /// Make sure mallets don't stay attached if VRKeys is disabled without
        /// calling Disable().
        /// </summary>
        private void OnDisable()
        {
            Disable();
        }

        /// <summary>
        /// Enable the keyboard.
        /// </summary>
        public void Enable()
        {
            if (!initialized)
            {
                // Make sure we're initialized first.
                StartCoroutine(EnableWhenInitialized());
                return;
            }

            disabled = false;

            if (keysParent != null)
            {
                keyboardHandle.SetActive(true);
                keysParent.gameObject.SetActive(true);
            }

            EnableInput();

            PositionAndAttachMallets();

            enable.Invoke();
        }

        private IEnumerator EnableWhenInitialized()
        {
            yield return new WaitUntil(() => initialized);

            Enable();
        }

        /// <summary>
        /// Disable the keyboard.
        /// </summary>
        public void Disable()
        {
            disabled = true;

            if (keysParent != null)
            {
                keyboardHandle.SetActive(false);
                keysParent.gameObject.SetActive(false);
            }

            DetachMallets();

            disable.Invoke();
        }

        /// <summary>
        /// Set the text value all at once.
        /// </summary>
        /// <param name="txt">New text value.</param>
        public void SetText(string txt)
        {
            if (activeInputField != null)
            {
                activeInputField.text = txt;
                OnUpdate.Invoke(txt);
            }
        }

        /// <summary>
        /// Add a character to the input text.
        /// </summary>
        /// <param name="character">Character.</param>
        public void AddCharacter(string character)
        {
            if (activeInputField != null)
            {
                activeInputField.text += character;
                OnUpdate.Invoke(activeInputField.text);

                if (shifted && character != "" && character != " ")
                {
                    StartCoroutine(DelayToggleShift());
                }
            }
        }

        /// <summary>
        /// Toggle whether the characters are shifted (caps).
        /// </summary>
        public bool ToggleShift()
        {
            if (keys == null)
            {
                return false;
            }

            shifted = !shifted;

            foreach (LetterKey key in keys)
            {
                key.shifted = shifted;
            }

            shiftKey.Toggle(shifted);

            return shifted;
        }

        private IEnumerator DelayToggleShift()
        {
            yield return new WaitForSeconds(0.1f);

            ToggleShift();
        }

        /// <summary>
        /// Disable keyboard input.
        /// </summary>
        public void DisableInput()
        {
            leftPressing = false;
            rightPressing = false;

            if (keys != null)
            {
                foreach (LetterKey key in keys)
                {
                    key?.Disable();
                }
            }

            foreach (Key key in extraKeys)
            {
                key.Disable();
            }
        }

        /// <summary>
        /// Re-enable keyboard input.
        /// </summary>
        public void EnableInput()
        {
            leftPressing = false;
            rightPressing = false;

            PositionAndAttachMallets();

            if (keys != null)
            {
                foreach (LetterKey key in keys)
                {
                    key?.Enable();
                }
            }

            foreach (Key key in extraKeys)
            {
                key.Enable();
            }
        }

        /// <summary>
        /// Backspace one character.
        /// </summary>
        public void Backspace()
        {
            if (activeInputField != null)
            {
                if (activeInputField.text.Length > 0)
                {
                    activeInputField.text = activeInputField.text[..^1];
                }

                OnUpdate.Invoke(activeInputField.text);
            }
        }

        /// <summary>
        /// Submit and close the keyboard.
        /// </summary>
        public void Submit()
        {
            if (activeInputField != null)
            {
                OnSubmit.Invoke(activeInputField.text);
                activeInputField = null;
            }
        }

        /// <summary>
        /// Cancel input and close the keyboard.
        /// </summary>
        public void Cancel()
        {
            activeInputField = null;
            OnCancel.Invoke();
        }

        /// <summary>
        /// Set the language of the keyboard.
        /// </summary>
        /// <param name="layout">New language.</param>
        public void SetLayout(KeyboardLayout layout)
        {
            StartCoroutine(DoSetLanguage(layout));
        }

        private IEnumerator DoSetLanguage(KeyboardLayout lang)
        {
            keyboardLayout = lang;
            layout = LayoutList.GetLayout(keyboardLayout);

            yield return StartCoroutine(SetupKeys());

            // Update extra keys
            foreach (Key key in extraKeys)
            {
                key.UpdateLayout(layout);
            }
        }

        /// <summary>
        /// Setup the keys.
        /// </summary>
        private IEnumerator SetupKeys()
        {
            bool activeState = keysParent.gameObject.activeSelf;

            // Hide everything before setting up the keys
            keysParent.gameObject.SetActive(false);

            // Remove previous keys
            if (keys != null)
            {
                foreach (Key key in keys)
                {
                    if (key != null)
                    {
                        Destroy(key.gameObject);
                    }
                }
            }

            keys = new LetterKey[layout.TotalKeys()];
            int keyCount = 0;

            // Numbers row
            for (int i = 0; i < layout.row1Keys.Length; i++)
            {
                GameObject obj = (GameObject)Instantiate(keyPrefab, keysParent);
                obj.transform.localPosition += (Vector3.right * ((keyWidth * i) - layout.row1Offset));

                LetterKey key = obj.GetComponent<LetterKey>();
                key.character = layout.row1Keys[i];
                key.shiftedChar = layout.row1Shift[i];
                key.shifted = false;
                key.Init(obj.transform.localPosition);

                obj.name = "Key: " + layout.row1Keys[i];
                obj.SetActive(true);

                keys[keyCount] = key;
                keyCount++;

                yield return null;
            }

            // QWERTY row
            for (int i = 0; i < layout.row2Keys.Length; i++)
            {
                GameObject obj = (GameObject)Instantiate(keyPrefab, keysParent);
                obj.transform.localPosition += (Vector3.right * ((keyWidth * i) - layout.row2Offset));
                obj.transform.localPosition += (Vector3.back * keyHeight * 1);

                LetterKey key = obj.GetComponent<LetterKey>();
                key.character = layout.row2Keys[i];
                key.shiftedChar = layout.row2Shift[i];
                key.shifted = false;
                key.Init(obj.transform.localPosition);

                obj.name = "Key: " + layout.row2Keys[i];
                obj.SetActive(true);

                keys[keyCount] = key;
                keyCount++;

                yield return null;
            }

            // ASDF row
            for (int i = 0; i < layout.row3Keys.Length; i++)
            {
                GameObject obj = (GameObject)Instantiate(keyPrefab, keysParent);
                obj.transform.localPosition += (Vector3.right * ((keyWidth * i) - layout.row3Offset));
                obj.transform.localPosition += (Vector3.back * keyHeight * 2);

                LetterKey key = obj.GetComponent<LetterKey>();
                key.character = layout.row3Keys[i];
                key.shiftedChar = layout.row3Shift[i];
                key.shifted = false;
                key.Init(obj.transform.localPosition);

                obj.name = "Key: " + layout.row3Keys[i];
                obj.SetActive(true);

                keys[keyCount] = key;
                keyCount++;

                yield return null;
            }

            // ZXCV row
            for (int i = 0; i < layout.row4Keys.Length; i++)
            {
                GameObject obj = (GameObject)Instantiate(keyPrefab, keysParent);
                obj.transform.localPosition += (Vector3.right * ((keyWidth * i) - layout.row4Offset));
                obj.transform.localPosition += (Vector3.back * keyHeight * 3);

                LetterKey key = obj.GetComponent<LetterKey>();
                key.character = layout.row4Keys[i];
                key.shiftedChar = layout.row4Shift[i];
                key.shifted = false;
                key.Init(obj.transform.localPosition);

                obj.name = "Key: " + layout.row4Keys[i];
                obj.SetActive(true);

                keys[keyCount] = key;
                keyCount++;

                yield return null;
            }

            // Reset visibility of keyboard
            keysParent.gameObject.SetActive(activeState);
        }
    }
}