using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyFight
{
    public class EventBus
    {
        private static EventBus instance;

        public static EventBus Instance
        {
            get
            {
                if (instance == null) instance = new EventBus();
                return instance;
            }
        }

        public MainEventsBus MainEvents { get; }
        public CamEventsBus CameraEvents { get; }
        public UiEventsBus UIEvents { get; }
        
        public EventBus()
        {
            MainEvents = new MainEventsBus();
            CameraEvents = new CamEventsBus();
            UIEvents = new UiEventsBus();
        }
        
        public class MainEventsBus
        {
            public Action GameInitialized;
            public Action Unsubscribe;
            public Action RestartGame;
            public Action GameStarted;
            public Action OnUpdate;
        }
        
        public class CamEventsBus
        {
            public Action<Transform> SetTarget;
            public Action SetRandomTarget;
            public Action<Transform> CheckIfNeedChangeTarget;
            public Action ZoomIn;
        }

        public class UiEventsBus
        {
            public Action ShowLevelCompleteScreen;
        } 
    }
}
