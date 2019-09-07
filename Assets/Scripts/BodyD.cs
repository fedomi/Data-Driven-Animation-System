using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyD 
{
    #region Attributes

    private Transform armature;
    private Transform hips;

    private Transform leftUpLeg;
    private Transform leftLeg;
    private Transform leftFoot;

    private Transform rightUpLeg;
    private Transform rightLeg;
    private Transform rightFoot;

    private Transform spine, spine1, spine2;
    private Transform leftShoulder;
    private Transform leftArm;
    private Transform leftForeArm;
    private Transform leftHand;

    private Transform rightShoulder;
    private Transform rightArm;
    private Transform rightForeArm;
    private Transform rightHand;

    private Transform neck;
    private Transform head;

    private List<Transform> joints;
    #endregion

    public BodyD(Transform t)
    {


        armature = t.GetChild(0);
        hips = armature.GetChild(0);
        leftUpLeg = hips.GetChild(0);
        leftLeg = leftUpLeg.GetChild(0);
        leftFoot = leftLeg.GetChild(0);

        rightUpLeg = hips.GetChild(1);
        rightLeg = rightUpLeg.GetChild(0);
        rightFoot = rightLeg.GetChild(0);

        spine = hips.GetChild(2);
        spine1 = spine.GetChild(0);
        spine2 = spine1.GetChild(0);

        leftShoulder = spine2.GetChild(0);
        leftArm = leftShoulder.GetChild(0);
        leftForeArm = leftArm.GetChild(0);
        leftHand = leftForeArm.GetChild(0);

        neck = spine2.GetChild(1);
        head = neck.GetChild(0);

        rightShoulder = spine2.GetChild(2);
        rightArm = rightShoulder.GetChild(0);
        rightForeArm = rightArm.GetChild(0);
        rightHand = rightForeArm.GetChild(0);

        joints = new List<Transform>();
        //joints.Add(armature);
        //joints.Add(hips);
        joints.Add(leftUpLeg);
        joints.Add(leftLeg);
        joints.Add(leftFoot);

        joints.Add(rightUpLeg);
        joints.Add(rightLeg);
        joints.Add(rightFoot);

        joints.Add(spine);
        joints.Add(spine1);
        joints.Add(spine2);

        joints.Add(leftShoulder);
        joints.Add(leftArm);
        joints.Add(leftForeArm);
        joints.Add(leftHand);

        joints.Add(neck);
        joints.Add(head);

        joints.Add(rightShoulder);
        joints.Add(rightArm);
        joints.Add(rightForeArm);
        joints.Add(rightHand);

    }

    #region Getters

    public Transform GetArmature() { return armature; }
    public Transform GetHips() { return hips; }

    public Transform GetLeftUpLeg() { return leftUpLeg; }
    public Transform GetLeftLeg() { return leftLeg; }
    public Transform GetLeftFoot() { return leftFoot; }

    public Transform GetRightUpLeg() { return rightUpLeg; }
    public Transform GetRightLeg() { return rightLeg; }
    public Transform GetRightFoot() { return rightFoot; }

    public Transform GetSpine() { return spine; }
    public Transform GetSpine1() { return spine1; }
    public Transform GetSpine2() { return spine2; }

    public Transform GetRightShoulder() { return rightShoulder; }
    public Transform GetRightArm() { return rightArm; }
    public Transform GetRightForeArm() { return rightForeArm; }
    public Transform GetRightHand() { return rightHand; }

    public Transform GetNeck() { return neck; }
    public Transform GetHead() { return head; }

    public Transform GetLeftShoulder() { return leftShoulder; }
    public Transform GetLeftArm() { return leftArm; }
    public Transform GetLeftForeArm() { return leftForeArm; }
    public Transform GetLeftHand() { return leftHand; }

    public List<Transform> GetJoints() { return joints; }
    #endregion


    public Vector3 GetFlatPosition() {

        Vector3 pos = GetHips().position;
        return new Vector3(pos.x, 0, pos.z);

    }

    public void PrintState()
    {
        Debug.Log("Hip Pos: " + hips.transform.position);

    }

    // Gets a list of joints positions and applies them to the body
    private void SetPositions(List<Vector3> j) {
        //armature.position = j[0];
        hips.position = j[1];

        leftUpLeg.position = j[2];
        leftLeg.position = j[3];
        leftFoot.position = j[4];

        rightUpLeg.position = j[5];
        rightLeg.position = j[6];
        rightFoot.position = j[7];

        spine.position = j[8];
        spine1.position = j[9];
        spine2.position = j[10];

        rightShoulder.position = j[11];
        rightArm.position = j[12];
        rightForeArm.position = j[13];
        rightHand.position = j[14];

        neck.position = j[15];
        head.position = j[16];

        leftShoulder.position = j[17];
        leftArm.position = j[18];
        leftForeArm.position = j[19];
        leftHand.position = j[20];
    }


    private void SetLocalPositions(List<Vector3> j)
    {
        //armature.position = j[0];
        //hips.position = j[1];
        /*0.110028,-0.0534519,-0.01599121*/
        leftUpLeg.position = hips.position + j[0];/*j[1];*/
        leftLeg.position = leftUpLeg.position + j[1];
        leftFoot.position = leftLeg.position  + j[2];
        
        rightUpLeg.position = hips.position + j[3];
        rightLeg.position = rightUpLeg.position + j[4];
        rightFoot.position = rightLeg.position + j[5];

        spine.position = hips.position + j[6];
        spine1.position = spine.position + j[7];
        spine2.position = spine1.position + j[8];

        leftShoulder.position = spine2.position + j[9];
        leftArm.position = leftShoulder.position + j[10];
        leftForeArm.position = leftArm.position + j[11];
        leftHand.position = leftForeArm.position + j[12];

        neck.position = spine2.position + j[13];
        head.position = neck.position + j[14];

        rightShoulder.position = spine2.position + j[15];
        rightArm.position = rightShoulder.position + j[16];
        rightForeArm.position = rightArm.position + j[17];
        rightHand.position = rightForeArm.position + j[18];
        



    }


    // Gets a list of joints rotations and applies them to the body
    private void SetRotatons(List<Vector3> h)
    {
        //armature.eulerAngles = h[0];
        hips.eulerAngles = h[1];

        leftUpLeg.eulerAngles = h[2];
        leftLeg.eulerAngles = h[3];
        leftFoot.eulerAngles = h[4];

        rightUpLeg.eulerAngles = h[5];
        rightLeg.eulerAngles = h[6];
        rightFoot.eulerAngles = h[7];

        spine.eulerAngles = h[8];
        spine1.eulerAngles = h[9];
        spine2.eulerAngles = h[10];

        rightShoulder.eulerAngles = h[11];
        rightArm.eulerAngles = h[12];
        rightForeArm.eulerAngles = h[13];
        rightHand.eulerAngles = h[14];

        neck.eulerAngles = h[15];
        head.eulerAngles = h[16];

        leftShoulder.eulerAngles = h[17];
        leftArm.eulerAngles = h[18];
        leftForeArm.eulerAngles = h[19];
        leftHand.eulerAngles = h[20];
    }

    // Mix the two previous methods
    public void ApplyPose(List<Vector3> j) {
        SetLocalPositions(j);
        //SetRotatons(h);

    }

}
