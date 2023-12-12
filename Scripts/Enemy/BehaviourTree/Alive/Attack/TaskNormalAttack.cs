using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskNormalAttack : BTNode
{
    private Animator animator;
    private Transform transform;
    private float attackRange;

    public TaskNormalAttack(BT bt)
    {
        this.bt = bt;
        root = bt.Root;
        transform = bt.transform;
        animator = bt.Anime;
        attackRange = bt.AttackRange;
    }

    public override eBTNodeState Evaluate()
    {
        Transform target = (Transform)root.GetData(StaticVariables.targetText);
        int combo = (int)root.GetData(StaticVariables.comboText);
        if(target != null)
        {
            ComboAttack(combo);
        }

        state = eBTNodeState.Running;
        return state;
    }

    private void ComboAttack(int combo)
    {
        bt.ResetAnimations();
        bt.Anime.Rebind();

        switch (combo)
        {
            case 1:
                {
                    animator.SetTrigger("Attack1");
                    break;
                }
            case 2:
                {
                    animator.SetTrigger("Attack2");
                    break;
                }
            case 3:
                {
                    animator.SetTrigger("Attack3");
                    break;
                }
            default:
                {
                    break;
                }
        }

        bt.IsDefensible = true;
    }
}
