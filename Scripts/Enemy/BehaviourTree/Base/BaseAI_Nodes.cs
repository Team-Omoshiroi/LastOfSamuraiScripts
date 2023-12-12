using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelDead : BTSelector
{
    public SelDead(BT bt)
    {
        AddChild(new SeqDead(bt));
    }
}

public class SeqDead : BTSequence
{
    public SeqDead(BT bt)
    {
        AddChild(new CheckDead(bt));
        AddChild(new TaskDead(bt));
    }
}

public class SelAlive : BTSelector
{
    public SelAlive(BT bt)
    {
        AddChild(new SeqAttack(bt));
        AddChild(new SeqChase(bt));
        AddChild(new SeqPatrol(bt));
    }
}

public class SeqAttack : BTSequence
{
    public SeqAttack(BT bt)
    {
        AddChild(new CheckAttackable(bt));
        AddChild(new SelAttack(bt));
    }
}

public class SelAttack : BTSelector
{
    public SelAttack(BT bt)
    {
        AddChild(new SeqSpecialAttack(bt));
        AddChild(new SeqNormalAttack(bt));
    }
}

public class SeqSpecialAttack : BTSequence
{
    public SeqSpecialAttack(BT bt)
    {
        AddChild(new CheckCoolDown_SA(bt));
        AddChild(new CheckFrequency_SA(bt));
        AddChild(new TaskSpecialAttack(bt));
    }
}

public class SeqNormalAttack : BTSequence
{
    public SeqNormalAttack(BT bt)
    {
        AddChild(new TaskNormalAttack(bt));
    }
}

public class SeqChase : BTSequence
{
    public SeqChase(BT bt)
    {
        AddChild(new CheckDetectPlayer(bt));
        AddChild(new TaskChase(bt));
    }
}

public class SeqPatrol : BTSequence
{
    public SeqPatrol(BT bt)
    {
        AddChild(new TaskPatrol(bt));
    }
}
