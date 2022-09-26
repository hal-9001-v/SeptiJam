using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimations : MonoBehaviour
{

    Animator animator => GetComponent<Animator>();

    const string WalkBoolKey = "Walking";
    const string GroundedBoolKey = "Grounded";
    const string WalkBlendKey = "WalkBlend";
    const string JumpTriggerKey = "Jump";
    const string CarTriggerKey = "Car";

    public void GetInCar()
    {
        animator.applyRootMotion = true;
        animator.SetTrigger(CarTriggerKey);
    }

    public void LeaveCar()
    {
        Jump();
    }

    public void Jump()
    {
        animator.SetTrigger(JumpTriggerKey);
    }

    public void StopWalking()
    {
        animator.SetBool(WalkBoolKey, false);
    }

    public void StartWalking()
    {
        animator.SetBool(WalkBoolKey, true);
    }

    public void SetGroundBool(bool b)
    {
        animator.SetBool(GroundedBoolKey, b);
    }

    public void SetWalkSpeed(float normalizedSpeed)
    {
        if (normalizedSpeed > 1) normalizedSpeed = 1;
        if (normalizedSpeed < 0) normalizedSpeed = 0;

        animator.SetFloat(WalkBlendKey, normalizedSpeed);
    }

}
