using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer
{
    internal class ServiceMonoBehaviour : MonoBehaviour
    {
        HashSet<MonoService> m_monoServices = new HashSet<MonoService>();
        HashSet<MonoService> m_monoUpdateServices = new HashSet<MonoService>();

        internal void Register(MonoService monoService)
        {
            m_monoServices.Add(monoService);
        }

        internal void BindUpdate(MonoService monoService)
        {
            m_monoUpdateServices.Add(monoService);
        }

        internal void UnbindUpdate(MonoService monoService)
        {
            if (m_monoUpdateServices.Contains(monoService))
            {
                m_monoUpdateServices.Remove(monoService);
            }
        }

        private void Update()
        {
            foreach (MonoService monoService in m_monoUpdateServices)
            {
                monoService.OnUpdate();
            }
        }

        private void OnDestroy()
        {
            foreach (var item in m_monoServices)
            {
                item.OnMonoDestroy();
            }
            m_monoServices.Clear();
            m_monoUpdateServices.Clear();
            m_monoServices = null;
            m_monoUpdateServices = null;
        }

    }
}