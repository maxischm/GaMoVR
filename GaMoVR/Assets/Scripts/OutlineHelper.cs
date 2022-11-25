using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class OutlineHelper : MonoBehaviour
{
    [SerializeField] private float outlineThickness = 10;

    private Outline _outline;
    // Start is called before the first frame update
    void Start()
    {
        _outline = GetComponent<Outline>();
    }

    [ContextMenu("Highlight")]
    public void Highlight()
    {
        _outline.OutlineWidth = outlineThickness;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
