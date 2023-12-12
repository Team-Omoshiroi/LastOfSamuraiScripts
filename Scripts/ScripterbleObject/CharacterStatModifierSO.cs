using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterStatModifierSO : ScriptableObject 
{
    public abstract void AffectCharacter(GameObject character, float val);//캐릭터 체력같은 구성요소와 같이 수정하려는 모든 구성요소에 액세스할수있도록 
}
