using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgentsExamples;
using Unity.MLAgents.Sensors;
using BodyPart = Unity.MLAgentsExamples.BodyPart;
using Random = UnityEngine.Random;

public class agent1 : Agent
{
    public GroundContact groundcontact;
    [Header("Object to pick up")] 
    public Transform pickup_object;
    public GameObject pickup_object_gameobject;  
    public Transform target;
    public Transform target_parent;

    [Header("Locations")] 
    [SerializeField] public List<Transform> number_of_locations;
    
    [Header("Arm Mover")] 
    public Transform middle_finger; 
    public Transform index_finger;

    public Transform ArmmoverTarget;
    public GameObject ArmmoverTarget_gameobject;

    private Rigidbody pickup_object_rbody;
    private Rigidbody target_rbody;
    private TrailRenderer trail_renderer;

    float step_count = 0f;

    void Start()
    {
        pickup_object_rbody = pickup_object.GetComponent<Rigidbody>();
        target_rbody = target_parent.GetComponent<Rigidbody>();
        trail_renderer = pickup_object.GetComponent<TrailRenderer>();
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log(step_count);
        step_count = 0f;

        groundcontact.touchingGround = false;
        destroy_joints();
        reset_everything();

        Globals.Episode +=1;
    }

    public float speed;

    void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];
        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 0.1f;
                break;
            case 2:
                dirToGo = transform.forward * -0.1f;
                break;
            case 3:
                dirToGo = transform.up * 0.1f;
                break;
            case 4:
                dirToGo = transform.up * -0.1f;
                break;
            case 5:
                dirToGo = transform.right * 0.1f;
                break;
            case 6:
                dirToGo = transform.right * -0.1f;
                break;
        }
        pickup_object_rbody.AddForce(dirToGo , ForceMode.VelocityChange);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        MoveAgent(actionBuffers.DiscreteActions);

        if (Vector3.Distance(pickup_object.transform.position, index_finger.transform.position) > 1.2f)
        {
            too_much_force();
        }        

        AddReward(-5f / MaxStep);

        step_count += 1f;
    }

    //***********************Collects observations**************************************************************************************************************************   
    private bool joint_made;
    public override void CollectObservations(VectorSensor sensor)
    {        
        if (pickup_object_gameobject.GetComponent<FixedJoint>() == null)
        {
            joint_made = false;
            sensor.AddObservation(joint_made);
            sensor.AddObservation(Vector3.Distance(pickup_object.transform.position, target_parent.transform.position));
        } 
        else
        {
            joint_made = true;
            sensor.AddObservation(joint_made);
            sensor.AddObservation(Vector3.Distance(pickup_object.transform.position, number_of_locations[0].transform.position));
        }
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

        if(pickup_object_gameobject.GetComponent<FixedJoint>() == null)
        {
            AddReward(-1.0f);
        }
        else
        {
            AddReward(-1.0f);
            destroy_joints();
        }
        EndEpisode();   
    }

    void reset_everything()
    {
        target_parent.localPosition = new Vector3(-0.8f, 0.84f, 1.94f);  // Static case for NN-2 -> Test case 1
        // target_parent.localPosition = new Vector3(-1.1f, 0.84f, 1.94f);  // Static case for NN-3 -> Test case 2
        // target_parent.localPosition = new Vector3(-1.3f, 0.84f, 1.94f);  // Static case for NN-4 -> Test case 3

        // target_parent.localPosition = new Vector3(Random.Range(-1.323f, -0.79f), 0.847f, 1.996f);  // Dynamic case for all NN's
        
        target_parent.localEulerAngles = new Vector3(-90, 0, -90);
        target_rbody.constraints = RigidbodyConstraints.FreezeAll;

        pickup_object.localPosition = new Vector3(-0.582f, 0.938f, 1.379f);
        pickup_object.localEulerAngles = new Vector3(0, 0, 0);
        pickup_object_rbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 0;
        
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 3;
        }
        else  if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 4;
        }
    }
 
}