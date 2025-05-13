using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionToggle : MonoBehaviour
{
    public float slowTimeScale = 0.2f;
    public KeyCode slowKey = KeyCode.LeftShift;
    private float originalFixedDeltaTime;
    [SerializeField] private PlayerMovement playerMovement;

    void Start()
    {
        originalFixedDeltaTime = Time.fixedDeltaTime;
    }

    void Update()
    {
        if (playerMovement.currentState != PlayerMovement.State.Fast && Input.GetKey(slowKey))
        {
            Time.timeScale = slowTimeScale;
            Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;
        }
        else
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = originalFixedDeltaTime;
        }
    }
}


