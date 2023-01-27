using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgentsExamples;
using Unity.MLAgents.Sensors;
using BodyPart = Unity.MLAgentsExamples.BodyPart;
using Random = UnityEngine.Random;

public class agent : Agent
{
    [Header("Object to pick up")] 
    public Transform pickup_object;
    public GameObject pickup_object_gameobject;  
    public Transform target;
    
    [Header("Locations")] 
    [SerializeField] public List<Transform> number_of_locations;
    
    [Header("Arm Mover")] 
    public Transform middle_finger; 
    public Transform index_finger;

    public Transform ArmmoverTarget;
    public GameObject ArmmoverTarget_gameobject;

    private Rigidbody pickup_object_rbody;
    private Rigidbody target_rbody;

    void Start()
    {
        pickup_object_rbody = pickup_object.GetComponent<Rigidbody>();
        target_rbody = target.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        destroy_joints();
        reset_everything();

        Globals.Episode +=1;
    }

    public float speed;
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        int moveX = actions.DiscreteActions[0]; 
        int moveY = actions.DiscreteActions[1];
        int moveZ = actions.DiscreteActions[2];

        

        Vector3 addForce = new Vector3(0, 0, 0);
        Vector3 rotate = new Vector3(0, 0, 0);
        // 0 = Dont move; 1 = left; ; 2 = right
        switch (moveX) 
        {
            case 0: addForce.x = 0f; break;
            case 1: addForce.x = -1f; break;
            case 2: addForce.x = 1f; break;
        }
        switch (moveY)
        {
            case 0: addForce.y = 0f; break;
            case 1: addForce.y = -1f; break;
            case 2: addForce.y = 1f; break;
        }
        switch (moveZ)
        {
            case 0: addForce.z = 0f; break;
            case 1: addForce.z = -1f; break;
            case 2: addForce.z = 1f; break;
        }

        

        pickup_object_rbody.velocity = addForce*speed;

        // if(pickup_object_gameobject.GetComponent<FixedJoint>() != null)
        // {
        //     int rotX = actions.DiscreteActions[3];
        //     int rotY = actions.DiscreteActions[4];
        //     int rotZ = actions.DiscreteActions[5];

        //     switch (rotX)
        //     {
        //         case 0: rotate.x = 0f; break;
        //         case 1: rotate.x = -1f; break;
        //         case 2: rotate.x = 1f; break;
        //     }
        //     switch (rotY)
        //     {
        //         case 0: rotate.y = 0f; break;
        //         case 1: rotate.y = -1f; break;
        //         case 2: rotate.y = 1f; break;
        //     }
        //     switch (rotZ)
        //     {
        //         case 0: rotate.z = 0f; break;
        //         case 1: rotate.z = -1f; break;
        //         case 2: rotate.z = 1f; break;
        //     }

        //     pickup_object.Rotate(rotate*speed);
        // }

        if(pickup_object.transform.localPosition.y < 1.0f)
        {
            fell_off_table();
        }

        if (Vector3.Distance(ArmmoverTarget.transform.position, middle_finger.transform.position) > 0.15f)
        {
            too_much_force();
        }
        

        // AddReward(-1f / MaxStep);
        // Debug.Log(MaxStep);
    }




    //***********************Collects 12 observations**************************************************************************************************************************   
    public override void CollectObservations(VectorSensor sensor)
    {        
        //Direction to target
        sensor.AddObservation(pickup_object.transform.localPosition); 
        sensor.AddObservation(target.transform.localPosition);

        sensor.AddObservation(number_of_locations[1].transform.localPosition);
        // sensor.AddObservation(number_of_locations[1].transform.localPosition);

        // sensor.AddObservation(Vector3.Distance(pickup_object.transform.localPosition, target.transform.localPosition)); //Distance between hand and object
        sensor.AddObservation(Vector3.Distance(target.transform.localPosition, number_of_locations[1].transform.localPosition)); //Distance between object and location // need to remove
        // sensor.AddObservation(Vector3.Distance(pickup_object.transform.localPosition, target.transform.localPosition) + Vector3.Distance(target.transform.localPosition, number_of_locations[0].transform.localPosition)); //Distance between object and location // need to remove

        sensor.AddObservation(pickup_object_rbody.velocity);
    }
    //***********************Remove the connected body from the joint and delete the joint**************************************************************************************
    void destroy_joints()
    {
        if(pickup_object_gameobject.GetComponent<FixedJoint>() != null)
        {
            pickup_object_gameobject.GetComponent<FixedJoint>().connectedBody = null;
            Destroy(pickup_object_gameobject.GetComponent<FixedJoint>());
        }
    }

    // ***********************Reward Functions *********************************************************************************************************************************
    void too_much_force()
    {        
        pickup_object_rbody.angularVelocity = Vector3.zero;
        pickup_object_rbody.velocity = Vector3.zero;
        Globals.Fail +=1;

        // if(pickup_object_gameobject.GetComponent<FixedJoint>() == null)
        // {
        //     AddReward(-1.0f);
        // }
        // else
        // {
            AddReward(-1.0f);
            // destroy_joints();
        // }
        EndEpisode();   
    }
    
    void fell_off_table()
    {
        AddReward(-1.0f);
        Globals.Fail +=1;
        // destroy_joints();
        EndEpisode();
    }

    void target_reached()
    {
        AddReward(1.0f);
        pickup_object_gameobject.AddComponent<FixedJoint>();
        pickup_object_gameobject.GetComponent<FixedJoint>().connectedBody = target_rbody;  
    }

    // void target_location_reached()
    // {
    //     AddReward(1.0f);
    //     Globals.Success +=1;
    //     // destroy_joints();
    //     EndEpisode();        
    // }

    void reset_everything()
    {
        // target.localPosition = new Vector3(-0.7f, 1.2f, 1.75f);
        // target.localEulerAngles = new Vector3(90, 0, 0);
        // target_rbody.constraints = RigidbodyConstraints.FreezeAll;

        pickup_object.position = number_of_locations[0].position;
        pickup_object.localEulerAngles = new Vector3(0, 0, 0);
        pickup_object_rbody.constraints = RigidbodyConstraints.FreezeRotation;

        // for (int i = 0; i < number_of_locations.Count; i++)
        // {
        //     number_of_locations[i].localEulerAngles = new Vector3(0, Random.Range(0.0f, 359.9f), 0);
        //     // number_of_locations[i].localPosition = new Vector3(Random.Range(-1.5f, -0.75f), 1.12f, Random.Range(1.64f, 1.89f));
        // }
    }

    //***********************Aligning the middle finger with 'pickup_object' position and 'index_finger' rotation******************************************************
    void FixedUpdate()
    {
        
        ArmmoverTarget.transform.position = pickup_object.transform.position; // Connects arm to the pickup object
        ArmmoverTarget.transform.rotation = index_finger.transform.rotation;


        // ArmmoverTarget.transform.rotation = pickup_object.transform.rotation;

        // Vector3 temp = ArmmoverTarget.transform.rotation.eulerAngles;
        // temp.x += 90.0f;
        // ArmmoverTarget.transform.rotation = Quaternion.Euler(temp);        
    }

    //*****************************************************************************************************************************************************************
    // public override void Heuristic(float[] actionsOut)
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        switch (Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")))
        {
            case -1: discreteActions[0] = 1; break;
            case 0: discreteActions[0] = 0; break;
            case 1: discreteActions[0] = 2; break;
        }
        switch (Mathf.RoundToInt(Input.GetAxisRaw("Vertical2")))
        {
            case -1: discreteActions[1] = 1; break;
            case 0: discreteActions[1] = 0; break;
            case 1: discreteActions[1] = 2; break;
        }
        switch (Mathf.RoundToInt(Input.GetAxisRaw("Vertical")))
        {
            case -1: discreteActions[2] = 1; break;
            case 0: discreteActions[2] = 0; break;
            case 1: discreteActions[2] = 2; break;
        }
        

        // actionsOut[0] = Input.GetAxis("Horizontal");
        // actionsOut[2] = Input.GetAxis("Vertical");
    }
 
}