using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Teamon.Tools.Screenshooter
{
	[CreateAssetMenu(fileName = "ScreenshooterSaveData", menuName = MenuPath, order = MenuOrder)]
	public class ScreenshooterSaveData : ScriptableObject
	{
		private const string MenuPath = "Team-on/Screenshooter/ScreenshooterSaveData";
		private const int MenuOrder = 10;
	
		private const string SAVE_FILE_PATH_DEFAULT = "Editor/ScreenShooter";
		private const string SAVE_FILE_NAME_DEFAULT = "ScreenShooterSettings.asset";

		public string outputFolder = "-----";

		public List<ScreenshotData> screenshoots = new List<ScreenshotData>(1)
		{
			new ScreenshotData("FullHD", new Vector2(1080, 1920)),
			new ScreenshotData("FullHD (No UI)", new Vector2(1080, 1920)) {captureOverlayUI = false},

			new ScreenshotData("iPhone 8 Plus", new Vector2(1242, 2208)),
			new ScreenshotData("iPhone 8 Plus (No UI)", new Vector2(1242, 2208)) {captureOverlayUI = false},

			new ScreenshotData("iPhone Xs Max", new Vector2(1242, 2688)),
			new ScreenshotData("iPhone Xs Max (No UI)", new Vector2(1242, 2688)) {captureOverlayUI = false},
		};

		public static void SaveSettings(ScreenshooterSaveData data)
		{
			var assetPath = UnityEditor.AssetDatabase.GetAssetPath(data);
			
			if (!string.IsNullOrEmpty(assetPath))
			{
				EditorUtility.SetDirty(data);
				UnityEditor.AssetDatabase.Refresh();
				return;
			}

			string diretoryPath = Path.Combine("Assets", SAVE_FILE_PATH_DEFAULT);

			if (!Directory.Exists(diretoryPath))
			{
				Directory.CreateDirectory(diretoryPath);
			}

			string filePath = Path.Combine(diretoryPath, SAVE_FILE_NAME_DEFAULT);
			UnityEditor.AssetDatabase.CreateAsset(data, filePath);
		}

		public static ScreenshooterSaveData LoadSettings()
		{
			string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(ScreenshooterSaveData).Name);
			if (guids.Length == 0)
			{
				Debug.LogWarning("Could not find ScreenshooterSaveData asset. Will use default settings instead.");
				return ScriptableObject.CreateInstance<ScreenshooterSaveData>();
			}
			else
			{
				string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
				return UnityEditor.AssetDatabase.LoadAssetAtPath<ScreenshooterSaveData>(path);
			}
		}
	}
}
