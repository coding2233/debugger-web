using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer
{
    public abstract class MonoService 
    {
        private static ServiceMonoBehaviour s_serviceGameObject;

        public MonoService()
        {
            if (s_serviceGameObject == null)
            {
                GameObject serviceGameObject = new GameObject("[Mono Service]");
                GameObject.DontDestroyOnLoad(serviceGameObject);
                //serviceGameObject.hideFlags = HideFlags.HideAndDontSave;
                s_serviceGameObject = serviceGameObject.AddComponent<ServiceMonoBehaviour>();
            }
            //注册
            s_serviceGameObject.Register(this);
            // 默认绑定
            BindUpdate(true);
        }

        protected Coroutine StartCoroutine(IEnumerator routine)
        {
            if (s_serviceGameObject == null)
                return null;

            return s_serviceGameObject.StartCoroutine(routine);
        }

        protected void Stop(IEnumerator routine)
        {
            if (s_serviceGameObject == null)
                return;

            s_serviceGameObject.StopCoroutine(routine);
        }

        protected void BindUpdate(bool bind)
        {
            if (s_serviceGameObject == null)
                return;

            if (bind)
            {
                s_serviceGameObject.BindUpdate(this);
            }
            else
            {
                s_serviceGameObject.UnbindUpdate(this);
            }
        }

        public virtual void OnUpdate()
        {
            
        }

        public virtual void OnMonoDestroy()
        {
            //s_serviceGameObject = null;
        }

    }
}