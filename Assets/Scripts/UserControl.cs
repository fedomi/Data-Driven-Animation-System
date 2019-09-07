using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserControl
{
    enum ACTIONS
    {
        Idle = 0,
        Walk = 1,
        Run = 2,   
        Stop = 3   
    }

    private Vector2 gp; // Goal position
    private Vector2 gq; // Goal facing direction
    private byte ga;    // Action to perform at goal (one-hot vector denoting action label)
    private float gt;   // Time to get to goal position

    public UserControl(Vector2 goalPos, Vector2 goalQ, byte action, float time) {
        gp = goalPos;
        gq = goalQ;
        ga = action;
        gt = time;
    }

}
