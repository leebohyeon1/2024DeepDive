using System.Collections;
using System.Collections.Generic; // ��ųʸ� Ŭ������ �����ؼ� �߰����� ��� Ŭ���� ���� ����
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EVENT_TYPE
{
    GAME_OVER,
    PRINCESS_ANGRY,
    WASH_DISHES,
    BABY_SEAT,
    COOK,
    STOP_INTERACT
};
public interface IListener
{
    void OnEvent(EVENT_TYPE Event_type, Component Sender, object Param = null);
}

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    public delegate void OnEvent(EVENT_TYPE Event_, Component Sender, object Param = null);
    private Dictionary<EVENT_TYPE, List<IListener>> Listeners = new Dictionary<EVENT_TYPE, List<IListener>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    // ������ �߰�
    public void AddListener(EVENT_TYPE Event_Type, IListener Listener)
    {
        if (Listeners.TryGetValue(Event_Type, out List<IListener> ListenList))
        {
            ListenList.Add(Listener);
        }
        else
        {
            ListenList = new List<IListener> { Listener };
            Listeners.Add(Event_Type, ListenList);
        }
    }

    // ���� ������ ����
    public void RemoveListener(EVENT_TYPE Event_Type, IListener Listener)
    {
        if (Listeners.TryGetValue(Event_Type, out List<IListener> ListenList))
        {
            ListenList.Remove(Listener);

            if (ListenList.Count == 0)
            {
                Listeners.Remove(Event_Type);
            }
        }
    }

    // ��� ������ ���� (�̺�Ʈ Ÿ�� ����)
    public void RemoveEvent(EVENT_TYPE Event_Type)
    {
        Listeners.Remove(Event_Type);
    }

    // �̺�Ʈ ����
    public void PostNotification(EVENT_TYPE Event_Type, Component Sender, object Param = null)
    {
        if (Listeners.TryGetValue(Event_Type, out List<IListener> ListenList))
        {
            for (int i = ListenList.Count - 1; i >= 0; i--)
            {
                ListenList[i]?.OnEvent(Event_Type, Sender, Param);
            }
        }
    }

    // �ߺ� ����
    public void RemoveRedundancies()
    {
        List<EVENT_TYPE> keysToRemove = new List<EVENT_TYPE>();

        foreach (var item in Listeners)
        {
            item.Value.RemoveAll(listener => listener == null);

            if (item.Value.Count == 0)
            {
                keysToRemove.Add(item.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            Listeners.Remove(key);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RemoveRedundancies();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
