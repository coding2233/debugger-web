using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Wanderer
{
    public class Toast : MonoBehaviour
    {

        private static Toast s_toast;

        private GameObject m_infoIcon;
        private GameObject m_warnIcon;
        private GameObject m_errorIcon;

        private Text m_showText;

        private Dictionary<ToastType, GameObject> m_toastIcons;
        private CanvasGroup m_canvasGroup;
        private ContentSizeFitter m_contentSizeFitter;
        private float m_showTime;

        [SerializeField]
        private Color[] m_toastColors=new Color[3] { Color.white, Color.white , Color.white };
        private string[] m_toastColorFormats;


        private void Awake()
        {
            var toastBackground = transform.Find("Background").gameObject;
            m_infoIcon = transform.Find("Background/Info").gameObject;
            m_warnIcon = transform.Find("Background/Warn").gameObject;
            m_errorIcon = transform.Find("Background/Error").gameObject;

            m_showText = transform.Find("Background/Text").GetComponent<Text>();

            m_toastIcons = new Dictionary<ToastType, GameObject>();
            m_toastIcons.Add(ToastType.Info, m_infoIcon);
            m_toastIcons.Add(ToastType.Warn, m_warnIcon);
            m_toastIcons.Add(ToastType.Error, m_errorIcon);

            m_canvasGroup = toastBackground.GetComponent<CanvasGroup>();
            m_contentSizeFitter = toastBackground.GetComponent<ContentSizeFitter>();
            gameObject.name = "[Toast]";
            s_toast = this;
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            if (m_canvasGroup.alpha == 1.0f)
            {
                m_showTime += Time.deltaTime;
                if (m_showTime >= 2.0f)
                {
                    m_canvasGroup.alpha = 0.0f;
                }
            }
        }


        private void SetIcon(ToastType toastType)
        {
            foreach (var item in m_toastIcons)
            {
                if (item.Key == toastType)
                {
                    item.Value.SetActive(true);
                }
                else
                {
                    item.Value.SetActive(false);
                }
            }
        }

        private void SetText(string text)
        {
            StartCoroutine(SetToastBackground(text));
        }
        private IEnumerator SetToastBackground(string text)
        {
            m_contentSizeFitter.gameObject.SetActive(true);
            m_showText.text = text;
            yield return new WaitForEndOfFrame();
            m_contentSizeFitter.enabled = false;
            yield return new WaitForEndOfFrame();
            m_contentSizeFitter.enabled = true;
            m_canvasGroup.alpha = 1.0f;
            m_showTime = 0.0f;
        }

        private string GetColorFormat(ToastType type)
        {
            if (m_toastColorFormats == null)
            {
                if (m_toastColors != null)
                {
                    m_toastColorFormats = new string[m_toastColors.Length];
                    for (int i = 0; i < m_toastColorFormats.Length; i++)
                    {
                        m_toastColorFormats[i] =$"#{ColorUtility.ToHtmlStringRGBA(m_toastColors[i])}";
                    }
                }
            }

            if (m_toastColorFormats != null)
            {
                int index = (int)type;
                if (index >= 0 && index < m_toastColorFormats.Length)
                {
                    return m_toastColorFormats[index];
                }
            }

            return "white";
        }

        public enum ToastType
        {
            Info=0,
            Warn,
            Error
        }
        public static void Show(string text, ToastType type = ToastType.Info)
        {
            if (s_toast == null)
            {
                s_toast = GameObject.Instantiate(Resources.Load<GameObject>("Canvas_Toast")).GetComponent<Toast>();
            }
            string textColor = s_toast.GetColorFormat(type);
            //if (type == ToastType.Warn)
            //{
            //    textColor = "yellow";
            //}
            //else if (type == ToastType.Error)
            //{
            //    textColor = "red";
            //}
            s_toast.SetText($"<color={textColor}>{text}</color>");
            s_toast.SetIcon(type);
        }
        public static void Info(string text)
        {
            Show(text, ToastType.Info);
        }
        public static void Warn(string text)
        {
            Show(text, ToastType.Warn);
        }

        public static void Error(string text)
        {
            Show(text, ToastType.Error);
        }

    }
}