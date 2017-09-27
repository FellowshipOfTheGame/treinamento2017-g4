using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TextPrint : MonoBehaviour {

    public TextAsset credits;
    public TextAsset credits_names;
    public TextAsset credits_names2;
    public Text Names;
    public Text Names2;
    public string previous_scene;

    void Start () {
        GetComponent<Text>().text = credits.text;
        Names.text = credits_names.text;
        Names2.text = credits_names2.text;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            //Application.LoadLevel(previous_scene);
            SceneManager.LoadScene(0);
        }
    }
 }
