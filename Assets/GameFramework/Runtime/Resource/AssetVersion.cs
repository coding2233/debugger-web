using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer
{
    [System.Serializable]
    public class AssetVersion
    {
        private int _hashCode = 0;
        /// <summary>
        /// 资源版本好
        /// </summary>
        public int Version = 0;
        //  public bool IsEncrypt = false;
        /// <summary>
        /// 当前的AppVersion
        /// </summary>
        public string AppVersion = "";
        /// <summary>
        /// 支持的老版本的
        /// </summary>
        public List<string> SupportOldAppVersions = new List<string>();
        /// <summary>
        /// 以前的资源路径链接
        /// </summary>
        public string OldResourceUrl = "";
        /// <summary>
        /// 资源信息
        /// </summary>
        public List<AssetHashInfo> AssetHashInfos = new List<AssetHashInfo>();
        /// <summary>
        /// ManifestAssetBundle
        /// </summary>
        public string Manifest;
        public override bool Equals(object obj)
        {
            AssetVersion other = obj as AssetVersion;
            if (Version != other.Version || other.GetHashCode() != GetHashCode())
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            if (_hashCode == 0)
            {
                _hashCode = JsonUtility.ToJson(this).GetHashCode();
            }
            return _hashCode;
        }
    }

    //资源hash值
    [System.Serializable]
    public class AssetHashInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;
        /// <summary>
        /// md5值
        /// </summary>
        public string MD5;
        /// <summary>
        /// KB
        /// </summary>
        public int Size;
        /// <summary>
        /// 更新标签, default/static/used , 默认更新/静默更新/使用时更新
        /// </summary>
        public string UpdateTag = "";
        /// <summary>
        /// 是否预加载
        /// </summary>
        public bool Preload = false;
        /// <summary>
        /// 所有的资源
        /// </summary>
        public List<string> Addressables=new List<string>();
        public override bool Equals(object obj)
        {
            AssetHashInfo other = obj as AssetHashInfo;
            if (other.Name.Equals(Name) && other.MD5.Equals(MD5))
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return (Name + MD5).GetHashCode();
        }

    }
}