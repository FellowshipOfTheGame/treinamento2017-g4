using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitOnClickz : MonoBehaviour
{

    public void Quit()
    {
        //se estiver usando o unity pra rodar (no editor)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
