using System.Collections;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoSave
{
    private static float m_LastEditorUpdateTime;
    [MenuItem("File/Save project %&s")]
    static void FunctionForceSaveProyect()
    {
        EditorApplication.ExecuteMenuItem("File/Save Project");
        Debug.Log("Saved project");
    }

    [MenuItem("File/Save Scene And Project %#&s")]
    static void FunctionForceSaveSceneAndProyect()
    {
        EditorApplication.ExecuteMenuItem("File/Save");
        EditorApplication.ExecuteMenuItem("File/Save Project");
        Debug.Log("Saved scene and project");
    }
    static AutoSave()
         {
             
             m_LastEditorUpdateTime = Time.realtimeSinceStartup;
             EditorApplication.update += OnEditorUpdate;
             
         }
     
         
         static void OnEditorUpdate()
         {
             float currentTime = Time.realtimeSinceStartup;
             if (currentTime - m_LastEditorUpdateTime >= 3 * 60)
             {
                 m_LastEditorUpdateTime = currentTime;
                 if (!EditorApplication.isPlayingOrWillChangePlaymode)
                 {
                     FunctionForceSaveSceneAndProyect();
                 }
                 
             }
         }
    
    



    
}