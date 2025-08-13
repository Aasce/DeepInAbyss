using Asce.Managers;
using UnityEngine;
using System;
using System.IO;

namespace Asce.Game.Players
{
    public class ScreenshotTaker : GameComponent
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F12)) // Press F12 to take screenshot
            {
                string folderPath = Path.Combine(Application.persistentDataPath, "screenshots");

                // Create folder if it doesn't exist
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string filename = Path.Combine(folderPath, $"{timestamp}.png");

                ScreenCapture.CaptureScreenshot(filename);
            }
        }
    }
}
