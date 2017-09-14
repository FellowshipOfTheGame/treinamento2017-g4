using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCtest : MonoBehaviour {
    private Animator anim;

    [SerializeField]
    private bool IsWalking;

    [SerializeField]
    private bool Left;

    [SerializeField]
    private bool Right;

    [SerializeField]
    private bool IsDead;

    [SerializeField]
    private bool HandsUp;

    private bool sameDir;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        anim.SetBool("IsWalking", IsWalking);
		if (Left && !sameDir) {
            anim.SetBool("Left", true);
            anim.SetBool("Right", false);
            Right = false;
            sameDir = true;
        }
        if (Right) {
            anim.SetBool("Right", true);
            anim.SetBool("Left", false);
            Left = false;
            sameDir = false;
        }
        anim.SetBool("IsDead", IsDead);
        anim.SetBool("HandsUp", HandsUp);
	}
}
