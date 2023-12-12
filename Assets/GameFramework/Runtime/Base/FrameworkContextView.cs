using strange.extensions.context.api;
using strange.extensions.context.impl;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Wanderer
{
    public class FrameworkContextView : ContextView
    {
        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);

            //重定向日志输出
            Log.Logger = new DefaultLog(Path.Combine(Application.persistentDataPath, "logs"));

            //框架初始化成功
            context = new FrameworkContext(this);
            Debug.Log("FrameworkContext Init.");

            //GameLaunchContext gameLaunchContext = new GameLaunchContext(this, ContextStartupFlags.MANUAL_LAUNCH);
            //context.AddContext(gameLaunchContext);
            //gameLaunchContext.Launch();
        }


        protected override void OnDestroy()
        {
            Log.Dispose();
            base.OnDestroy();
            context = null;
        }
    }

}