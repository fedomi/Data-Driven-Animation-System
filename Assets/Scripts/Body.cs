using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body 
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

    public Body(Transform t)
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
        /*leftUpLeg.position = hips.position + j[0];
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
        rightHand.position = rightForeArm.position + j[18];*/

        Vector3 root = GetHips().position;

        leftUpLeg.position = root + j[0];
        leftLeg.position = root + j[1];
        leftFoot.position = root + j[2];

        rightUpLeg.position = root + j[3];
        rightLeg.position = root + j[4];
        rightFoot.position = root + j[5];

        spine.position = root + j[6];
        spine1.position = root + j[7];
        spine2.position = root + j[8];

        leftShoulder.position = root + j[9];
        leftArm.position = root + j[10];
        leftForeArm.position = root + j[11];
        leftHand.position = root + j[12];

        neck.position = root + j[13];
        head.position = root + j[14];

        rightShoulder.position = root + j[15];
        rightArm.position = root + j[16];
        rightForeArm.position = root + j[17];
        rightHand.position = root + j[18];


    }

    private void SetLocalPositions(List<Vector3> j, float dAngle)
    {
        //armature.position = j[0];
        //hips.position = j[1];
        /*0.110028,-0.0534519,-0.01599121*/
        /*leftUpLeg.position = hips.position + j[0];
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
        rightHand.position = rightForeArm.position + j[18];*/

        Vector3 root = GetHips().position;

        leftUpLeg.position = root + RotatedLocalPosition(j[0],dAngle);
        leftLeg.position = root + RotatedLocalPosition(j[1], dAngle);
        leftFoot.position = root + RotatedLocalPosition(j[2], dAngle);

        rightUpLeg.position = root + RotatedLocalPosition(j[3], dAngle);
        rightLeg.position = root + RotatedLocalPosition(j[4], dAngle);
        rightFoot.position = root + RotatedLocalPosition(j[5], dAngle);

        spine.position = root + RotatedLocalPosition(j[6], dAngle);
        spine1.position = root + RotatedLocalPosition(j[7], dAngle);
        spine2.position = root + RotatedLocalPosition(j[8], dAngle);

        leftShoulder.position = root + RotatedLocalPosition(j[9], dAngle);
        leftArm.position = root + RotatedLocalPosition(j[10], dAngle);
        leftForeArm.position = root + RotatedLocalPosition(j[11], dAngle);
        leftHand.position = root + RotatedLocalPosition(j[12], dAngle);

        neck.position = root + RotatedLocalPosition(j[13], dAngle);
        head.position = root + RotatedLocalPosition(j[14], dAngle);

        rightShoulder.position = root + RotatedLocalPosition(j[15], dAngle);
        rightArm.position = root + RotatedLocalPosition(j[16], dAngle);
        rightForeArm.position = root + RotatedLocalPosition(j[17], dAngle);
        rightHand.position = root + RotatedLocalPosition(j[18], dAngle);


    }


    public Vector3 RotatedLocalPosition(Vector3 jp, float a) {
        float hipsFwd = hips.rotation.eulerAngles.y-90;

        Vector3 res = Quaternion.Euler(0, hipsFwd, 0) * jp;
        //Debug.LogWarning("Rotating joint " + hipsFwd + " degrees from " + jp.ToString("F8") + " to " + res.ToString("F8"));

        return res;
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


    public void ApplyPose(List<Vector3> j)
    {
        SetLocalPositions(j);
        //SetRotatons(h);

    }

    // Mix the two previous methods
    public void ApplyPose(List<Vector3> j, Vector2 rv, float deltaAngle) {
        Vector2 new_rv = new Vector2(rv.x, rv.y);
        MoveHip(new_rv);
        RotateHip(deltaAngle);
        SetLocalPositions(j, deltaAngle);
        //SetRotatons(h);

    }


    public void MoveHip(Vector2 rv) {
        Debug.LogWarning("Applying movement: " + rv.ToString("F4"));
        hips.position = AngleAndDistanceToPoint(rv, hips.position, hips.forward);

    }

    public void RotateHip(float a) {
        Vector3 prevRotation = hips.rotation.eulerAngles;
        //hips.rotation = Quaternion.AngleAxis(a,Vector3.up);
        hips.Rotate(0,a,0);
        Debug.LogWarning("Applying hip rotation " + a + " from " + prevRotation.ToString("F4") + " to " + hips.rotation.eulerAngles.ToString("F4"));
        Debug.DrawRay(hips.position, hips.forward, Color.yellow);
    }

    private Vector3 AngleAndDistanceToPoint(Vector2 p_info, Vector3 pivot, Vector3 fwd)
    {
        Vector3 point = pivot + fwd * p_info.y;

        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(new Vector3(0, p_info.x, 0)) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }
}
