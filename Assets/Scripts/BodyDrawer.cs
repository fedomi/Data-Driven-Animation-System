using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyDrawer : MonoBehaviour
{

    private Body b;
    public Bone bonePrefab;
    private List<Bone> bones;
    

    private void Start()
    {
        b = new Body(transform);
        bones = new List<Bone>();
        CreateBones();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBones();
    }

    void CreateBones() {
        Bone b0 = Instantiate(bonePrefab);
        b0.SetParameters(b.GetHips().position, b.GetLeftUpLeg().position);
        bones.Add(b0);

        Bone b1 = Instantiate(bonePrefab);
        b1.SetParameters(b.GetLeftUpLeg().position, b.GetLeftLeg().position);
        bones.Add(b1);

        Bone b2 = Instantiate(bonePrefab);
        b2.SetParameters(b.GetLeftLeg().position, b.GetLeftFoot().position);
        bones.Add(b2);

        Bone b3 = Instantiate(bonePrefab);
        b3.SetParameters(b.GetHips().position, b.GetRightUpLeg().position);
        bones.Add(b3);


        Bone b4 = Instantiate(bonePrefab);
        b4.SetParameters(b.GetRightUpLeg().position, b.GetRightLeg().position);
        bones.Add(b4);

        Bone b5 = Instantiate(bonePrefab);
        b5.SetParameters(b.GetRightLeg().position, b.GetRightFoot().position);
        bones.Add(b5);

        Bone b6 = Instantiate(bonePrefab);
        b6.SetParameters(b.GetHips().position, b.GetSpine().position);
        bones.Add(b6);

        Bone b7 = Instantiate(bonePrefab);
        b7.SetParameters(b.GetSpine().position, b.GetSpine1().position);
        bones.Add(b7);

        Bone b8 = Instantiate(bonePrefab);
        b8.SetParameters(b.GetSpine1().position, b.GetSpine2().position);
        bones.Add(b8);

        Bone b9 = Instantiate(bonePrefab);
        b9.SetParameters(b.GetSpine2().position, b.GetNeck().position);
        bones.Add(b9);

        Bone b10 = Instantiate(bonePrefab);
        b10.SetParameters(b.GetNeck().position, b.GetHead().position);
        bones.Add(b10);

        Bone b11 = Instantiate(bonePrefab);
        b11.SetParameters(b.GetSpine2().position, b.GetLeftShoulder().position);
        bones.Add(b11);

        Bone b12 = Instantiate(bonePrefab);
        b12.SetParameters(b.GetLeftShoulder().position, b.GetLeftArm().position);
        bones.Add(b12);

        Bone b13 = Instantiate(bonePrefab);
        b13.SetParameters(b.GetLeftArm().position, b.GetLeftForeArm().position);
        bones.Add(b13);

        Bone b14 = Instantiate(bonePrefab);
        b14.SetParameters(b.GetLeftForeArm().position, b.GetLeftHand().position);
        bones.Add(b14);

        Bone b15 = Instantiate(bonePrefab);
        b15.SetParameters(b.GetSpine2().position, b.GetRightShoulder().position);
        bones.Add(b15);

        Bone b16 = Instantiate(bonePrefab);
        b16.SetParameters(b.GetRightShoulder().position, b.GetRightArm().position);
        bones.Add(b16);

        Bone b17 = Instantiate(bonePrefab);
        b17.SetParameters(b.GetRightArm().position, b.GetRightForeArm().position);
        bones.Add(b17);

        Bone b18 = Instantiate(bonePrefab);
        b18.SetParameters(b.GetRightForeArm().position, b.GetRightHand().position);
        bones.Add(b18);


    }

    void UpdateBones() {
        bones[0].SetParameters(b.GetHips().position, b.GetLeftUpLeg().position);
        bones[1].SetParameters(b.GetLeftUpLeg().position, b.GetLeftLeg().position);
        bones[2].SetParameters(b.GetLeftLeg().position, b.GetLeftFoot().position);
        bones[3].SetParameters(b.GetHips().position, b.GetRightUpLeg().position);
        bones[4].SetParameters(b.GetRightUpLeg().position, b.GetRightLeg().position);
        bones[5].SetParameters(b.GetRightLeg().position, b.GetRightFoot().position);
        bones[6].SetParameters(b.GetHips().position, b.GetSpine().position);
        bones[7].SetParameters(b.GetSpine().position, b.GetSpine1().position);
        bones[8].SetParameters(b.GetSpine1().position, b.GetSpine2().position);
        bones[9].SetParameters(b.GetSpine2().position, b.GetNeck().position);
        bones[10].SetParameters(b.GetNeck().position, b.GetHead().position);
        bones[11].SetParameters(b.GetSpine2().position, b.GetLeftShoulder().position);
        bones[12].SetParameters(b.GetLeftShoulder().position, b.GetLeftArm().position);
        bones[13].SetParameters(b.GetLeftArm().position, b.GetLeftForeArm().position);
        bones[14].SetParameters(b.GetLeftForeArm().position, b.GetLeftHand().position);
        bones[15].SetParameters(b.GetSpine2().position, b.GetRightShoulder().position);
        bones[16].SetParameters(b.GetRightShoulder().position, b.GetRightArm().position);
        bones[17].SetParameters(b.GetRightArm().position, b.GetRightForeArm().position);
        bones[18].SetParameters(b.GetRightForeArm().position, b.GetRightHand().position);
    }
}
