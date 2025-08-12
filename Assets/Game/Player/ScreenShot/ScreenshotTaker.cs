using Asce.Managers;
using UnityEngine;
using System;

namespace Asce.Game.Players
{
    public class ScreenshotTaker : GameComponent
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F12)) // Press F12 to take screenshot
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string filename = $"{Application.persistentDataPath}/screenshots/{timestamp}.png";
                ScreenCapture.CaptureScreenshot(filename);
                Debug.Log("Screenshot saved to: " + filename);
            }
        }
    }
}
