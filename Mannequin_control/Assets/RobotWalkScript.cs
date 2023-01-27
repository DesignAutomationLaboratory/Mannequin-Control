using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using TMPro;

public class RobotWalkScript : MonoBehaviour
{
    private Animator anim;
    public agent1 cumAgent;
    public TextMeshPro cumulativeReawardText;
 
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // float h = Input.GetAxis("Horizontal");
        // anim.SetFloat("Direction", h);
        // float v = Input.GetAxis("Vertical");
        // anim.SetFloat("Speed", v);
        cumulativeReawardText.text = cumAgent.GetCumulativeReward().ToString();
    }
}
