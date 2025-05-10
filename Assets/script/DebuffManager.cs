using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class DebuffManager : MonoBehaviour
{
    public enum State
    {
        Normal, // 기본
        Slow,   // 슬로우
        Revert, // 방향키 반전
        Ice     // 빙판
    }


    private Character characterController;
    public State currentState = State.Normal;


    private void Awake()
    {
        characterController = GetComponent<Character>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Normal:
                SetNormal();
                break;
            case State.Slow:
                OnSlow();
                break;
            case State.Revert:
                OnRevert();
                break;
            case State.Ice:
                OnIce();
                break;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor_Slow")
        {
            currentState = State.Slow;
        }
        else if (collision.gameObject.tag == "Floor_Revert")
        {
            currentState = State.Revert;
        }
        else if (collision.gameObject.tag == "Floor_Ice")
        {
            currentState = State.Ice;
        }
        else if (collision.gameObject.tag == "Floor_Default")
        {
            currentState = State.Normal;
        }
    }

    private void SetNormal()
    {

    }
    private void OnSlow()
    {

    }
    private void OnRevert()
    {

    }
    private void OnIce()
    {

    }
}
