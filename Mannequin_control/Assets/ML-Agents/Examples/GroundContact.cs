using UnityEngine;
using Unity.MLAgents;
using System.Collections.Generic;
using Unity.MLAgents.Policies;

namespace Unity.MLAgentsExamples
{
    /// <summary>
    /// This class contains logic for locomotion agents with joints which might make contact with the ground.
    /// By attaching this as a component to those joints, their contact with the ground can be used as either
    /// an observation for that agent, and/or a means of punishing the agent for making undesirable contact.
    /// </summary>
    [DisallowMultipleComponent]
    public class GroundContact : MonoBehaviour
    {
        public Agent agent;

        // public Agent agent_secondbrain;

        public bool touchingGround;
        public bool createjoint;
        // public bool touchingBody;        

        const string k_Ground = "ground"; // Tag of ground object.
        const string k_Body = "Body";
        const string k_Target = "Target";
        const string k_Location = "Location";
        const string k_Detector1 = "Detector1";
        const string k_Detector2 = "Detector2";
        const string k_pick_up = "pick_up";

        public GameObject pickup_object_gameobject;
        public Transform pickup_object;
        public Transform target;
        public Transform robotkyle;

        Vector3 lastPosition;
        
        [Header("Locations")] 
        [SerializeField] public List<Transform> number_of_locations;
        
        private Rigidbody pickup_object_rbody;
        private Rigidbody target_rbody;

        void Start()
        {
            pickup_object_rbody = pickup_object.GetComponent<Rigidbody>();
            target_rbody = target.GetComponent<Rigidbody>();
            lastPosition = pickup_object.transform.position;
        }       
      
            void OnCollisionEnter(Collision col)
            {
            // Checks only for the hand cube
            if (createjoint == true) 
            {
                // Hand touching the cylinder and create a joint 
                if (col.collider.CompareTag(k_Target) && pickup_object_gameobject.GetComponent<FixedJoint>() == null) 
                {
                    agent.AddReward(2.0f);

                    target_rbody.constraints = RigidbodyConstraints.None;  // setting all the degrees of freedom free for the red cylinder 
                    pickup_object_gameobject.AddComponent<FixedJoint>();
                    pickup_object_gameobject.GetComponent<FixedJoint>().connectedBody = target_rbody;                    
                }
            }

            if (col.collider.CompareTag(k_Ground) == true)
            {
                agent.AddReward(-0.1f);
            }
        }

        void OnTriggerEnter(Collider other) 
        {  
            if(other.CompareTag(k_Detector1) && (this.CompareTag(k_Target) || this.CompareTag(k_pick_up)))
            {
                pickup_object_rbody.velocity = new Vector3(0, 0, 0);
                agent.AddReward(2.5f);  
                destroy_joints();
                agent.EndEpisode(); 
            }
        }

        void destroy_joints()
        {
            if(pickup_object_gameobject.GetComponent<FixedJoint>() != null)
            {
                pickup_object_gameobject.GetComponent<FixedJoint>().connectedBody = null;
                Destroy(pickup_object_gameobject.GetComponent<FixedJoint>());
            }
        }
    }
}
