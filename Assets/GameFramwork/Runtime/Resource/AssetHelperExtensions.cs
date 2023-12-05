using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Wanderer
{
    public static class AssetHelperExtensions 
    {
		public static Task<T> LoadAsset<T>(this IAssetsHelper assetsHelper, string assetPath) where T : UnityEngine.Object
		{
			var taskResult =new TaskCompletionSource<T>();
			assetsHelper.LoadAsset<T>(assetPath, (result) => { taskResult.SetResult(result); });
			return taskResult.Task;
		}

		public static Task<T[]> LoadAllAsset<T>(this IAssetsHelper assetsHelper, string assetPath) where T : UnityEngine.Object
		{
			var taskResult = new TaskCompletionSource<T[]>();
			assetsHelper.LoadAllAsset<T>(assetPath, (result) => { taskResult.SetResult(result); });
			return taskResult.Task;
		}
	}
}