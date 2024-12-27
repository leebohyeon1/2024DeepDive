using System.Collections;
using System.Collections.Generic; // 딕셔너리 클래스를 포함해서 추가적인 모노 클래스 접근 가능
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EVENT_TYPE
{
    GAME_OVER,
    PRINCESS_ANGRY
};
public interface IListener
{
    void OnEvent(EVENT_TYPE Event_type, Component Sender, object Param = null);
}

public class EventManager : Singleton<EventManager>
{
    //=========================================================

    public delegate void OnEvent(EVENT_TYPE Event_, Component Sender, object Param = null);
    private Dictionary<EVENT_TYPE, List<IListener>> Listeners =
        new Dictionary<EVENT_TYPE, List<IListener>>();
    //=========================================================

    //리스트에 리스너 오브젝트 추가
    public void AddListener(EVENT_TYPE Event_Type, IListener Listener)
    {
        List<IListener> ListenList = null;

        if (Listeners.TryGetValue(Event_Type, out ListenList))
        {
            ListenList.Add(Listener);
            return;
        }


        ListenList = new List<IListener>();
        ListenList.Add(Listener);
        Listeners.Add(Event_Type, ListenList);
    }

    //이벤트를 리스너에게 전달한다.
    public void PostNotification(EVENT_TYPE Event_Type, Component Sender, object Param = null)
    {
        List<IListener> ListenList = null;

        if (!Listeners.TryGetValue(Event_Type, out ListenList)) { return; }

        for (int i = 0; i < ListenList.Count; i++)
        {
            if (!ListenList[i].Equals(null))
            {
                ListenList[i].OnEvent(Event_Type, Sender, Param);
            }
        }
    }

    //이벤트 종류와 리스너 항목을 딕셔너리에서 지운다.
    public void RemoveEvenet(EVENT_TYPE Event_Type)
    {
        Listeners.Remove(Event_Type);
    }

    //딕셔너리에 필요없는 항목들을 제거
    public void RemoveRedundancies()
    {
        Dictionary<EVENT_TYPE, List<IListener>> TmpListeners =
            new Dictionary<EVENT_TYPE, List<IListener>>();

        foreach (KeyValuePair<EVENT_TYPE, List<IListener>> Item in Listeners)
        {
            for (int i = Item.Value.Count - 1; i >= 0; i--)
            {
                if (Item.Value[i].Equals(null))
                {
                    Item.Value.RemoveAt(i);
                }
            }

            if (Item.Value.Count == 0)
            {
                TmpListeners.Add(Item.Key, Item.Value);
            }

            Listeners = TmpListeners;
        }
    }

    //씬이 변경되면 호출.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RemoveRedundancies(); //딕셔너리 청소
    }

    protected override void OnEnable()
    {
        // SceneManager.sceneLoaded 이벤트에 OnSceneLoaded 메서드를 구독.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void OnDisable()
    {
        // SceneManager.sceneLoaded 이벤트에서 OnSceneLoaded 메서드를 구독 해제.
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}