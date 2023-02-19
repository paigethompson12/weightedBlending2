using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocker : Kinematic
{
    public bool avoidObstacles = false;
    public GameObject myPursueTarget;
    
    Kinematic[] kSheep;

    BlendedSteering mySteering;
    PrioritySteering advancedSteering;

    void Start()
    {
        // Separate from other Sheep
        Separation separate = new Separation();
        separate.character = this;

        GameObject[] goSheep = GameObject.FindGameObjectsWithTag("sheep");
        kSheep = new Kinematic[goSheep.Length-1];

        int indx = 0;
        for (int i=0; i<goSheep.Length-1; i++)
        {
            if (goSheep[i] == this)
            {
                continue;
            }
            kSheep[indx++] = goSheep[i].GetComponent<Kinematic>();
        }
        separate.targets = kSheep;

        // Pursue the center of mass
        Arrive pursue = new Arrive();
        pursue.character = this;
        pursue.target = myPursueTarget;

        // look where center of mass is going
        LookWhereGoing myRotateType = new LookWhereGoing();
        myRotateType.character = this;

        mySteering = new BlendedSteering();

        // set up prioritysteering
        ObstacleAvoidance avoid = new ObstacleAvoidance();
        avoid.character = this;
        avoid.target = myPursueTarget;
        avoid.flee = true; // otherwise I seek to the obstacle
        BlendedSteering highPrioritySteering = new BlendedSteering();
        highPrioritySteering.behaviors = new BehaviorAndWeight[1];
        highPrioritySteering.behaviors[0] = new BehaviorAndWeight();
        highPrioritySteering.behaviors[0].behavior = avoid;
        highPrioritySteering.behaviors[0].weight = 1f;

        advancedSteering = new PrioritySteering();
        advancedSteering.groups = new BlendedSteering[2];
        advancedSteering.groups[0] = new BlendedSteering();
        advancedSteering.groups[0] = highPrioritySteering;
        advancedSteering.groups[1] = new BlendedSteering();
        advancedSteering.groups[1] = mySteering;

        // do the behavior and weights
        mySteering.behaviors = new BehaviorAndWeight[3];
        mySteering.behaviors[0] = new BehaviorAndWeight();
        mySteering.behaviors[0].behavior = separate;
        mySteering.behaviors[0].weight = 1f;
        mySteering.behaviors[1] = new BehaviorAndWeight();
        mySteering.behaviors[1].behavior = pursue;
        mySteering.behaviors[1].weight = 1f;
        mySteering.behaviors[2] = new BehaviorAndWeight();
        mySteering.behaviors[2].behavior = myRotateType;
        mySteering.behaviors[2].weight = 1f;

        
    }

    // Update is called once per frame
    protected override void Update()
    {

        steeringUpdate = new SteeringOutput();
        if (!avoidObstacles)
        {
            steeringUpdate = mySteering.getSteering();
        }
        else
        {
            steeringUpdate = advancedSteering.getSteering();
        }
        base.Update();
    }
}