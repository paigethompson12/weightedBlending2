using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorAndWeight
{
    public SteeringBehavior behavior = null;
    public float weight = 0f;
}

public class BlendedSteering
{

    public BehaviorAndWeight[] behaviors;

    // the overall maximum acceleration and rotation
    float maxAcceleration = 1f;
    float maxRotation = 5f;

    public SteeringOutput getSteering()
    {
        SteeringOutput result = new SteeringOutput();

        // accumulate all accelrations
        foreach (BehaviorAndWeight b in behaviors)
        {
            SteeringOutput s = b.behavior.getSteering();
            if (s != null)
            {
                result.angular += s.angular * b.weight;
                result.linear += s.linear * b.weight;
            }
        }

        // crop the result and return
        // need to use vector instead of float
        result.linear = result.linear.normalized * maxAcceleration;
        float angularAcceleration = Mathf.Abs(result.angular);
        if (angularAcceleration > maxRotation)
        {
            result.angular /= angularAcceleration;
            result.angular *= maxRotation;
        }

        return result;
    }

}