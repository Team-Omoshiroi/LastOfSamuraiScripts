using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public interface IDestroyableItem
    {

    }

    public interface IItemAction
    {
        public string ActionName { get; }
        bool PerformAction(GameObject character);

    }

public interface IEventNotifier
{
    void Notify(string message);
}

public interface IEneumy
{
    int ID { get; set; }
}

