using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    MarbleMazeAgent agent;
    // Start is called before the first frame update

    void Start()
    {
        // H1 -> Holes -> InnerFrame -> OuterFrame
        agent = transform.parent.parent.parent.GetComponent<MarbleMazeAgent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        string otherName = other.gameObject.transform.name;
        if (otherName.Equals("Marble"))
        {
            agent.AddReward(MarbleMazeAgent.REWARD_HOLE);
            agent.EndEpisodeCustom();
        }

    }
}
