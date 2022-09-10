using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public class AnimatorData : IComponentData
{
    public RuntimeAnimatorController animatorController;
    public Animator animator;
}
