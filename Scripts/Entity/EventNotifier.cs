using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventNotifier : MonoBehaviour
{
    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private Transform notificationParent;
    [SerializeField] private int maxActiveNotifications = 5; // 최대 활성 알림 개수

    private Queue<GameObject> notificationPool; // 사용 가능한 알림의 오브젝트 풀
    private Queue<GameObject> activeNotifications; // 활성화된 알림들을 추적하는 큐

    // 싱글톤 인스턴스를 위한 정적 변수
    public static EventNotifier Instance { get; private set; }

    private void Awake()
    {
        // 싱글톤 초기화
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject); // 중복 인스턴스 제거
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

    // 싱글톤 인스턴스를 통해 호출할 수 있는 메서드
    public void Notify(string message)
    {
        if (activeNotifications.Count >= maxActiveNotifications)
        {
            // 최대 개수에 도달했다면, 가장 오래된 알림을 비활성화하고 풀로 반환합니다.
            GameObject oldNotification = activeNotifications.Dequeue();
            oldNotification.SetActive(false);
            notificationPool.Enqueue(oldNotification);
        }

        // 새 알림을 표시합니다.
        if (notificationPool.Count > 0)
        {
            GameObject notification = notificationPool.Dequeue();
            notification.GetComponentInChildren<TextMeshProUGUI>().text = message;
            notification.SetActive(true);
            activeNotifications.Enqueue(notification);
        }
    }
}