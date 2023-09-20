using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Box : MonoBehaviour
{
    MarbleMazeAgent agent;
    TextMeshPro hoursDisplay;
    TextMeshPro episodesDisplay;

    // Start is called before the first frame update
    void Start()
    {
        // InnerFrame -> OuterFrame
        agent = transform.Find("OuterFrame").gameObject.GetComponent<MarbleMazeAgent>();
        hoursDisplay = transform.Find("Hours").gameObject.GetComponent<TextMeshPro>();
        episodesDisplay = transform.Find("Episodes").gameObject.GetComponent<TextMeshPro>();

    }


    // Update is called once per frame
    void Update()
    {
        episodesDisplay.text = MarbleMazeAgent.totalEpisodes.ToString();
        TimeSpan timeSpan = TimeSpan.FromSeconds(Time.fixedUnscaledTime);
        hoursDisplay.text = string.Format("{0:D1}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }
}
