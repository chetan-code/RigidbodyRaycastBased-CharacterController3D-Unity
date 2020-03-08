using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIK : MonoBehaviour
{

    Animator anim;

    Transform leftFoot;
    Transform rightFoot;

    Transform rightShoulder;
    Transform leftShoulder;

    Vector3 leftFoot_pos;
    Vector3 rightFoot_pos;

    Quaternion leftFoot_rot;
    Quaternion rightFoot_rot;

    Vector3 leftHand_pos;
    Vector3 rightHand_pos;

    Quaternion leftHand_rot;
    Quaternion rightHand_rot;

    float leftFoot_Weight;
    float rightFoot_Weight;

    float leftHand_Weight;
    float rightHand_Weight;

    public Transform lookAtThis;

    private void Start()
    {
        anim = GetComponent<Animator>();
        leftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
        leftShoulder = anim.GetBoneTransform(HumanBodyBones.LeftShoulder);
        rightShoulder = anim.GetBoneTransform(HumanBodyBones.RightShoulder);
    }
    private void OnAnimatorIK(int layerIndex)
    {
        // foot IK
        leftFoot_Weight = anim.GetFloat("LeftFoot");
        rightFoot_Weight = anim.GetFloat("RightFoot");
        // find raycast positions
        FindFloorPositions(leftFoot, ref leftFoot_pos, ref leftFoot_rot, Vector3.up);
        FindFloorPositions(rightFoot, ref rightFoot_pos, ref rightFoot_rot, Vector3.up);

        // replace the weights with leftFoot_Weight, and rightFoot_Weight when you've set them in your animation's Curves
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);

        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);

        // set the position of the feet
        anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftFoot_pos);
        anim.SetIKPosition(AvatarIKGoal.RightFoot, rightFoot_pos);

        // set the rotation of the feet
        anim.SetIKRotation(AvatarIKGoal.LeftFoot, leftFoot_rot);
        anim.SetIKRotation(AvatarIKGoal.RightFoot, rightFoot_rot);

        // hand IK

        // find raycast positions
        FindFloorPositions(leftShoulder, ref leftHand_pos, ref leftHand_rot, -transform.forward);
        FindFloorPositions(rightShoulder, ref rightHand_pos, ref rightHand_rot, -transform.forward);

        // distance between hands and raycast hit
        float distanceRightArmObject = Vector3.Distance(anim.GetBoneTransform(HumanBodyBones.RightShoulder).position, rightHand_pos);
        float distanceLeftArmObject = Vector3.Distance(anim.GetBoneTransform(HumanBodyBones.LeftShoulder).position, leftHand_pos);
        // blend weight based on the distance
        leftHand_Weight = Mathf.Clamp01(1 - distanceLeftArmObject);
        rightHand_Weight = Mathf.Clamp01(1 - distanceRightArmObject);


        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHand_Weight);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHand_Weight);

        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHand_Weight);
        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHand_Weight);

        // set the position of the hand
        anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHand_pos);
        anim.SetIKPosition(AvatarIKGoal.RightHand, rightHand_pos);

        // set the rotation of the hand
        anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHand_rot);
        anim.SetIKRotation(AvatarIKGoal.RightHand, rightHand_rot);
        // head IK
        if (lookAtThis != null)
        {
            // distance between face and object to look at
            float distanceFaceObject = Vector3.Distance(anim.GetBoneTransform(HumanBodyBones.Head).position, lookAtThis.position);

            anim.SetLookAtPosition(lookAtThis.position);
            // blend based on the distance
            anim.SetLookAtWeight(Mathf.Clamp01(2 - distanceFaceObject), Mathf.Clamp01(1 - distanceFaceObject));
        }
    }

    void FindFloorPositions(Transform t, ref Vector3 targetPosition, ref Quaternion targetRotation, Vector3 direction)
    {
        RaycastHit hit;
        Vector3 rayOrigin = t.position;
        // move the ray origin back a bit
        rayOrigin += direction * 0.3f;

        // raycast in the given direction
        Debug.DrawRay(rayOrigin, -direction, Color.green);
        if (Physics.Raycast(rayOrigin, -direction, out hit, 3))
        {
            // the hit point is the position of the hand/foot
            targetPosition = hit.point;
            // then rotate based on the hit normal
            Quaternion rot = Quaternion.LookRotation(transform.forward);
            targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * rot;
        }
    }
}