﻿using System;
using Common;
using Screeps3D;
using UnityEngine;

namespace Screeps_API
{
    [RequireComponent(typeof(ScreepsHTTP))]
    [RequireComponent(typeof(ScreepsSocket))]
    public class ScreepsAPI : BaseSingleton<ScreepsAPI>
    {
        public static ServerCache Cache { get; /*private*/ set; }
        public static ScreepsHTTP Http { get; private set; }
        public static ScreepsSocket Socket { get; private set; }
        public static ScreepsUser Me { get; private set; }
        public static BadgeManager Badges { get; private set; }
        public static UserManager UserManager { get; private set; }
        public static CpuMonitor Monitor { get; private set; }
        public static ScreepsConsole Console { get; private set; }
        public static long Time { get; internal set; }
        public static bool IsConnected { get; private set; }
        
        public static event Action<bool> OnConnectionStatusChange;
        public static event Action<long> OnTick;
        public static event Action OnShutdown;

        private string _token;

        public void Awake()
        {
            base.Awake();

            Http = GetComponent<ScreepsHTTP>();
            Socket = GetComponent<ScreepsSocket>();
            Badges = GetComponent<BadgeManager>();
            Monitor = GetComponent<CpuMonitor>();
            Console = GetComponent<ScreepsConsole>();
            UserManager = new UserManager();
        }

        // Use this for initialization
        public void Connect(ServerCache cache)
        {
            Cache = cache;
            
            // configure HTTP
            Http.Auth(o =>
            {
                NotifyText.Message("Success", Color.green, 1);
                Socket.Connect();
                Http.GetUser(AssignUser);
            }, () =>
            {
                Debug.Log("login failed");
            });
        }

        public void Disconnect()
        {
            SetConnectionStatus(false);
            Badges.Reset();
            UserManager.Reset();
            Socket.Disconnect();
        }

        internal void IncrementTime()
        {
            Time++;
            if (OnTick != null)
                OnTick(Time);
        }

        internal void AuthFailure()
        {
            SetConnectionStatus(false);
        }

        private void AssignUser(string str)
        {
            var obj = new JSONObject(str);
            Me = UserManager.CacheUser(obj);

            SetConnectionStatus(true);
            
            Http.Request("GET", "/api/game/time", null, SetTime);
        }

        private void SetTime(string obj)
        {
            var timeData = new JSONObject(obj)["time"];
            if (timeData != null)
            {
                Time = (int) timeData.n;
            }
        }

        private void OnDestroy()
        {
            if (OnShutdown != null)
                OnShutdown();
        }

        private void SetConnectionStatus(bool isConnected)
        {
            IsConnected = isConnected;
            if (OnConnectionStatusChange != null) OnConnectionStatusChange(isConnected);
        }
    }
}