using System;
using System.Collections.Generic;
using eHub.Android.Models;

namespace eHub.Android.Observables
{
    internal class MainMenuHandler
    {
        List<IObserver<MenuItem>> _observers;

        public MainMenuHandler()
        {
            _observers = new List<IObserver<MenuItem>>();
        }

        public IDisposable Subscribe(IObserver<MenuItem> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            return null;
        }
    }


}