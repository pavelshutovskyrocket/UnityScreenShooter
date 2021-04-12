﻿using System;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Teamon.Tools.Screenshooter
{
	public class ScreenShooterWindow : EditorWindow
	{
		ScreenshooterSaveData data;

		Vector2 scrollPos;
		ReorderableList _list;

		[MenuItem("Window/Screen Shooter &s")]
		public static void ShowWindow()
		{
			GetWindow(typeof(ScreenShooterWindow), false, "Screen Shooter", true);
		}

		private void Awake()
		{
			LoadSettings();
		}

		private void OnDestroy()
		{
			SaveSettings();
		}

		private void OnGUI()
		{
			if (data == null)
			{
				LoadSettings();
			}
			
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

			_list = _list ?? ScreenShooterConfigList.Create(data.screenshoots, OnMenuItemAdd);
			_list.DoLayoutList();

			GUI.enabled = !Application.isPlaying;
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Load Settings"))
			{
				LoadSettings();
			}
			if (GUILayout.Button("Save Settings"))
			{
				SaveSettings();
			}
			GUILayout.EndHorizontal();

			GUI.enabled = true;
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			if (data.outputFolder == "-----")
				data.outputFolder = ScreenshotTaker.GetDefaultScreenshotPath();
			data.outputFolder = PathField("Save to:", data.outputFolder);

			GUI.enabled = Application.isPlaying;

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(ScreenshotTaker.IsPaused ? "Play" : "Pause"))
			{
				ScreenshotTaker.Pause();
			}
			if (GUILayout.Button("Next Frame"))
			{
				ScreenshotTaker.WaitOneFrame();
			}
			GUILayout.EndHorizontal();

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			GUI.enabled = Application.isPlaying && ScreenshotTakerEditor.isScreenshotQueueEmpty;
			
			if (GUILayout.Button("Capture Screenshots Series"))
			{
				ScreenshotTakerEditor.CaptureScreenshootQueueAllLanguages(data.screenshoots, data.outputFolder);
			}
			if (GUILayout.Button("Take Game View Screenshot"))
			{
				ScreenshotTaker.TakeScreenshot(data.outputFolder);
			}

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			GUI.enabled = !Application.isPlaying && ScreenshotTakerEditor.isScreenshotQueueEmpty;
			
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Add all sizes to Game window sizes selector"))
			{
				ScreenshotTakerEditor.AddAllSizes(data.screenshoots);
			}

			if (GUILayout.Button("Clear all Game window sizes selector"))
			{
				ScreenshotTakerEditor.ClearAllSizes();
			}
			EditorGUILayout.EndHorizontal();

			GUI.enabled = true;
			EditorGUILayout.EndScrollView();
		}

		private void LoadSettings()
		{
			data = ScreenshooterSaveData.LoadSettings();
		}

		private void SaveSettings()
		{
			ScreenshooterSaveData.SaveSettings(data);
		}

		private void OnMenuItemAdd(object target)
		{
			data.screenshoots.Add(target as ScreenshotData);
		}

		private string PathField(string label, string path)
		{
			GUILayout.BeginHorizontal();
			path = EditorGUILayout.TextField(label, path);
			if (GUILayout.Button("...", GUILayout.Width(25f)))
			{
				string selectedPath = EditorUtility.OpenFolderPanel("Choose output directory", "", "");
				if (!string.IsNullOrEmpty(selectedPath))
					path = selectedPath;

				GUIUtility.keyboardControl = 0; // Remove focus from active text field
			}

			if (GUILayout.Button("Open", GUILayout.Width(100)))
			{
				var file = Directory.EnumerateFiles(data.outputFolder).FirstOrDefault();
				if (!string.IsNullOrEmpty(file))
					EditorUtility.RevealInFinder(Path.Combine(data.outputFolder, file));
				else
					EditorUtility.RevealInFinder(data.outputFolder);
			}

			GUILayout.EndHorizontal();

			return path;
		}
	}
}
