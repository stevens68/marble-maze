using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;


public class MarbleMazeAgent : Agent
{

    const int NO_ACTION = 0;  // do nothing
    const int LEFT = 1; // turn left-right-wheel left
    const int RIGHT = 2; // turn left-right-wheel right
    const int UP = 1; // turn up-down-wheel up
    const int DOWN = 2; // turn up-down-wheel down

    const float MAX_ANGLE_EULER = 1.5f; // max angle of wheels / panes
    const float ROTATION_STEP_EULER = 0.1f; // rotation per action

    public const float REWARD_STEP = -0.005f; // penalty for not being quick
    public const float REWARD_HOLE = -5f; // penalty for hitting a hole (worse than timeout)
    public const float REWARD_CP_HIT = 10f; // reward for hitting a checkpoint

    const float MAX_SECONDS_SINCE_LAST_CHECKPOINT = 6; // timeout when not reaching the next checkpoint
    const float START_OFFSET = 0.03f; // random offset to not alwaysw start on exactly the same spot

    GameObject innerFrame; 
    GameObject marble;

    public static int totalEpisodes; // episode counter 

    public float secondsSinceLastCheckpoint; // we reset it to 0 whenever the next checkpoint is reached

    //public TrainingRecorder rec;

    public override void Initialize()
    {
        
        innerFrame = transform.Find("InnerFrame").gameObject; 
        marble = innerFrame.transform.Find("Marble").gameObject;

        totalEpisodes = 0;

        //rec = Camera.main.GetComponent<TrainingRecorder>();
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        discreteActions[0] = NO_ACTION;
        discreteActions[1] = NO_ACTION;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            discreteActions[0] = UP;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            discreteActions[0] = DOWN;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            discreteActions[1] = LEFT;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            discreteActions[1] = RIGHT;
        }

    }


    public override void OnEpisodeBegin()
    {

        base.OnEpisodeBegin();

        /*
        if (totalEpisodes == 0)
        {
            rec.StartRecording(totalEpisodes);
        }
        else if (totalEpisodes == 8000000)
        {
            rec.StopRecording();
        }
        */

        totalEpisodes++;

        // reset frames
        innerFrame.transform.localRotation = new Quaternion(0f, 0f,0f, 0f);
        transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);

        // initially, marble is not moving
        marble.GetComponent<Rigidbody>().velocity = Vector3.zero;
        marble.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        // reset checkpoints
        foreach (Transform child in innerFrame.transform.Find("Checkpoints").transform)
        {
            CheckPoint cp = child.gameObject.GetComponent<CheckPoint>();
            cp.Reset();
        }

        Transform fromCheckpoint = innerFrame.transform.Find("Checkpoints").GetChild(0);
        marble.transform.position = fromCheckpoint.position + new Vector3(Random.Range(-START_OFFSET, START_OFFSET), 0.5f, Random.Range(-START_OFFSET, START_OFFSET));
        

        secondsSinceLastCheckpoint = 0f;
        
    }
    

    public override void OnActionReceived(ActionBuffers actions)
    {

        AddReward(REWARD_STEP);

        int upDown = actions.DiscreteActions[0];
        int leftRight = actions.DiscreteActions[1];

        if (leftRight == LEFT && (transform.rotation.eulerAngles.z + 180f) % 360 < 180 + MAX_ANGLE_EULER)
        {
            this.transform.eulerAngles += new Vector3(0, 0, ROTATION_STEP_EULER);
            
        }
        else if (leftRight == RIGHT && (transform.rotation.eulerAngles.z + 180f) % 360 > 180 - MAX_ANGLE_EULER)
        {
            this.transform.eulerAngles += new Vector3(0, 0, -ROTATION_STEP_EULER);
        }

        if (upDown == UP && (innerFrame.transform.rotation.eulerAngles.x + 180f) % 360 < 180 + MAX_ANGLE_EULER)
        {
            innerFrame.transform.eulerAngles += new Vector3(ROTATION_STEP_EULER, 0, 0);
        }
        else if (upDown == DOWN && (innerFrame.transform.rotation.eulerAngles.x + 180f) % 360 > 180 - MAX_ANGLE_EULER)
        {
            innerFrame.transform.eulerAngles += new Vector3(-ROTATION_STEP_EULER, 0, 0);
        }


        secondsSinceLastCheckpoint += Time.deltaTime;
        if (secondsSinceLastCheckpoint >= MAX_SECONDS_SINCE_LAST_CHECKPOINT)
        {
            EndEpisodeCustom();
        }


    }


    public void EndEpisodeCustom()
    {
        EndEpisode();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // localPosition of Sphere 
        sensor.AddObservation(marble.transform.localPosition);
        // velocity of Sphere 
        sensor.AddObservation(marble.GetComponent<Rigidbody>().velocity);
        // angular velocity of Sphere 
        sensor.AddObservation(marble.GetComponent<Rigidbody>().angularVelocity);
    }
}
