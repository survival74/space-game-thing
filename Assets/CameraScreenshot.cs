using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CameraScreenshot : MonoBehaviour
{
	public int resWidth = 1920;
	public int resHeight = 1080;

	public bool takeShot = false;
	private new Camera camera; 


	public static string ScreenshotName(int width, int height)
	{
		return string.Format("{0}/Screenshots/screen{1}x{2}{3}.png",
						 Application.dataPath,
						 width, height,
						 System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
	}

	private void Awake()
	{
		if (camera == null) camera = GetComponent<Camera>();
		if (camera == null) camera = Camera.main;
	}

	public void TakeScreenshot()
	{
		takeShot = true;
	}

	void LateUpdate()
	{
		takeShot |= Input.GetKeyDown("k");
		if (takeShot)
		{
			RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
			camera.targetTexture = rt;
			Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
			camera.Render();
			RenderTexture.active = rt;

			screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);

			camera.targetTexture = null;
			RenderTexture.active = null; // JC: added to avoid errors
			Destroy(rt);

			byte[] bytes = screenShot.EncodeToPNG();
			string filename = ScreenshotName(resWidth, resHeight);
			System.IO.File.WriteAllBytes(filename, bytes);
			Debug.Log(string.Format("Took screenshot to: {0}", filename));

			takeShot = false;
		}
	}
}
