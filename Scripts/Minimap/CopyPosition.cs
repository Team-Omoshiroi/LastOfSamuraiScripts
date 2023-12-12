using UnityEngine;

public class CopyPosition : MonoBehaviour
{
	[SerializeField]
	private bool x, y, z;   // �� ���� true�̸� target�� ��ǥ, false�̸� ���� ��ǥ�� �״�� ���
	[SerializeField]
	private Transform target;     //Ÿ�ٴ��

	private void Update()
	{
		if (!target) return;

		transform.position = new Vector3(
			(x ? target.position.x : transform.position.x),
			(y ? target.position.y : transform.position.y),
			(z ? target.position.z : transform.position.z));
	}
}