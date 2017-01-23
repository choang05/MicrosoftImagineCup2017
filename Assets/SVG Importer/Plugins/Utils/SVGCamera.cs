// Copyright (C) 2015 Jaroslav Stehlik - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;
using System.Collections;

namespace SVGImporter.Utils
{
	[RequireComponent(typeof(Camera))]
	public class SVGCamera : MonoBehaviour 
	{
		public static System.Action<Camera> onPreRender;
		protected Camera _camera;
new		public Camera camera
		{
			get {
				if(_camera == null) _camera = GetComponent<Camera>();
				return _camera;
			}
		}

		void OnPreRender()
		{
			if(onPreRender != null) onPreRender(camera);
		}

		public static void UpdateCameras()
		{
			Camera[] cameras = Camera.allCameras;
			for(int i = 0; i < cameras.Length; i++)
			{
				if(cameras[i] == null) continue;
				if(cameras[i].GetComponent<SVGCamera>() != null) continue;
				cameras[i].gameObject.AddComponent<SVGCamera>();
			}

#if UNITY_EDITOR
			if(!Application.isPlaying)
			{
				if(onPreRender != null) onPreRender(Camera.current);
			}
#endif
		}
	}
}
