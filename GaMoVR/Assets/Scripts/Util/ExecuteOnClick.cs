using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ExecuteOnClick : MonoBehaviour
{
    public CustomUIButton button;

    public bool executeOnstart;
    // Start is called before the first frame update
    void Start()
    {
        if (executeOnstart)
            StartCoroutine(DelayRoomCreation());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Execute!")]
    public void ExecutingOnClick()
    {
        button.onClick.Invoke();
    }

    private IEnumerator DelayRoomCreation()
    {
        yield return new WaitForSeconds(2);
        ExecutingOnClick();
    }
}
