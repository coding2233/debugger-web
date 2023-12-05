using strange.extensions.context.api;
using strange.extensions.context.impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer
{
    public class FrameworkContext : MVCSContext
    {

        public FrameworkContext(ContextView contextView) : base(contextView)
        {
            Log.Info("FrameworkContext");
        }


        protected override void mapBindings()
        {
            base.mapBindings();

            Debug.Log($"FrameworkContext mapBindings {contextView}");
            //事件
            //crossContextBridge.Bind(GameEvent.Entrance);

            //跨Context的 的service或者Model
            injectionBinder.Bind<IWebRequest>().To<WebRequestService>().ToSingleton().CrossContext();
            //injectionBinder.Bind<INetwork>().To<NetworkService>().ToSingleton().CrossContext();
#if UNITY_EDITOR
    #if EDITOR_BUNDLE
            injectionBinder.Bind<IAssetsHelper>().To<BundleAssetsHelperService>().ToSingleton().CrossContext();
    #else
            injectionBinder.Bind<IAssetsHelper>().To<EditorAssetsHelperService>().ToSingleton().CrossContext();
    #endif
#else
            injectionBinder.Bind<IAssetsHelper>().To<BundleAssetsHelperService>().ToSingleton().CrossContext();
#endif


            commandBinder.Bind(ContextEvent.START).To<FrameworkStartCommand>().Once();
        }

        protected override void instantiateCoreComponents()
        {
            base.instantiateCoreComponents();

            //injectionBinder.Bind<GameObject>().ToValue(m_contextView.transform.Find("CanvasRoot").gameObject).ToName(FramworkContextKeys.CanvasRoot).CrossContext();
            //injectionBinder.Bind<GameObject>().ToValue(m_contextView.transform.Find("HotUpdate").gameObject).ToName(FramworkContextKeys.HotUpdate).CrossContext();
        }

        //public void GameEntrance()
        //{
        //    dispatcher.Dispatch(GameEvent.Entrance);
        //}

    }
}