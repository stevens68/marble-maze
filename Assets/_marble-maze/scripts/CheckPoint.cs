using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    MarbleMazeAgent agent;
    bool hit;


    public void Reset()
    {
        hit = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        // CP (01) -> Checkpoints -> InnerFrame -> OuterFrame
        agent = transform.parent.parent.parent.GetComponent<MarbleMazeAgent>();
        Reset();
    }


    private void OnTriggerEnter(Collider other)
    {
        string otherName = other.gameObject.transform.name;
        if (otherName.Equals("Marble"))
        {
            if (! hit)
            {
                agent.secondsSinceLastCheckpoint = 0f;
                hit = true;
                Debug.Log(otherName + " hit " + transform.name);
                agent.AddReward(MarbleMazeAgent.REWARD_CP_HIT);
            }

            if (transform.name.Equals("FINISH"))
            {
                agent.EndEpisodeCustom();
            }
        }

    }
}
