using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CharControl : MonoBehaviour
{
    public static CharControl Instance;

    private PlayableDirector pd;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        pd = GetComponent<PlayableDirector>();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "CutScene")
        {
            other.gameObject.SetActive(false);
            pd.Play();
        }
    }
}
