using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskSpecialAttack : BTNode
{
    private Animator animator;
    private Transform transform;

    public TaskSpecialAttack(BT bt)
    {
        this.bt = bt;
        animator = bt.Anime;
        transform = bt.transform;
        root = bt.Root;
    }

    public override eBTNodeState Evaluate()
    {
        Transform target = (Transform)root.GetData(StaticVariables.targetText);
        if (target != null)
        {
            bt.ResetAnimations();
            bt.Anime.Rebind();
            animator.SetTrigger("SpecialAttack");
            Debug.Log("SpeacialAttack!!!");
            root.ClearData(StaticVariables.comboText);
            root.SetData(StaticVariables.saCooldownText, false);
            bt.IsUnstoppable = true;
        }

        state = eBTNodeState.Running;
        return state;
    }
}
