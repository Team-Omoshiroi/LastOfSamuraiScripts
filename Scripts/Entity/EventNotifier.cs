using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventNotifier : MonoBehaviour
{
    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private Transform notificationParent;
    [SerializeField] private int maxActiveNotifications = 5; // �ִ� Ȱ�� �˸� ����

    private Queue<GameObject> notificationPool; // ��� ������ �˸��� ������Ʈ Ǯ
    private Queue<GameObject> activeNotifications; // Ȱ��ȭ�� �˸����� �����ϴ� ť

    // �̱��� �ν��Ͻ��� ���� ���� ����
    public static EventNotifier Instance { get; private set; }

    private void Awake()
    {
        // �̱��� �ʱ�ȭ
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject); // �ߺ� �ν��Ͻ� ����
            return;
        }
        Instance = this;

        notificationPool = new Queue<GameObject>();
        activeNotifications = new Queue<GameObject>();

        for (int i = 0; i < maxActiveNotifications; i++)
        {
            GameObject instance = Instantiate(notificationPrefab, notificationParent);
            instance.SetActive(false);
            notificationPool.Enqueue(instance);
        }
    }

    // �̱��� �ν��Ͻ��� ���� ȣ���� �� �ִ� �޼���
    public void Notify(string message)
    {
        if (activeNotifications.Count >= maxActiveNotifications)
        {
            // �ִ� ������ �����ߴٸ�, ���� ������ �˸��� ��Ȱ��ȭ�ϰ� Ǯ�� ��ȯ�մϴ�.
            GameObject oldNotification = activeNotifications.Dequeue();
            oldNotification.SetActive(false);
            notificationPool.Enqueue(oldNotification);
        }

        // �� �˸��� ǥ���մϴ�.
        if (notificationPool.Count > 0)
        {
            GameObject notification = notificationPool.Dequeue();
            notification.GetComponentInChildren<TextMeshProUGUI>().text = message;
            notification.SetActive(true);
            activeNotifications.Enqueue(notification);
        }
    }
}