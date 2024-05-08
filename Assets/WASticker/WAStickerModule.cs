using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

namespace WASticker
{
    public class WAStickerModule : MonoBehaviour
    {
        const string pluginName = "com.lemon.wasticker.MainPlugin";

        private static AndroidJavaClass _pluginClass;
        private static AndroidJavaObject _pluginInstance;
        private static AndroidJavaObject currentActivity;

        public static AndroidJavaClass PluginClass
        {
            get
            {
                if (_pluginClass == null)
                {
                    _pluginClass = new AndroidJavaClass(pluginName);
                }
                return _pluginClass;
            }
        }

        public static AndroidJavaObject PluginInstance
        {
            get
            {
                if (_pluginInstance == null)
                {
                    _pluginInstance = PluginClass.CallStatic<AndroidJavaObject>("getInstance");
                }
                return _pluginInstance;
            }
        }

        private static AndroidJavaObject GetCurrentActivity()
        {
            if (currentActivity == null)
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
            return currentActivity;
        }

        void Start()
        {
        }

        public int GetLibraryStatus()
        {
            //just demo
            int status = 0;
            try
            {
                status = PluginInstance.Call<int>("getPluginStatus");
            }
            catch (System.Exception e)
            {
                Debug.Log("Exception " + e);
                status = -1;
            }

            return status;

        }

        public T CallFunctionFromPlugin<T>(string functionName, params object[] parameters)
        {
            return PluginInstance.Call<T>(functionName, parameters);
        }

        public void CallFunctionFromPlugin(string functionName, params object[] parameters)
        {
            PluginInstance.Call(functionName, parameters);
        }

        public void ToastMessage(string message)
        {
            CallFunctionFromPlugin("toastMessage", GetCurrentActivity(), message);
        }

        public void SetStatusBarTransparent()
        {
            CallFunctionFromPlugin("setStatusBarTransparent", GetCurrentActivity());
        }

        public void SetStickerInformation(string identifier, string stickerPackName, string publisher, string imageDataVersion, string trayIconPath, params string[] stickersPath)
        {
            CallFunctionFromPlugin("SetStickerInformation",GetCurrentActivity() , identifier, stickerPackName, publisher, imageDataVersion, trayIconPath, StickerPathToJson(stickersPath));
        }

        public bool ApplyStickerToWhatsapp(string identifier, string stickerPackName)
        {
            return CallFunctionFromPlugin<bool>("ApplyStickerFromActivity", GetCurrentActivity(), identifier, stickerPackName);
        }

        public string GetStickerFolder()
        {
            return CallFunctionFromPlugin<string>("getStickerFolder", GetCurrentActivity());
        }

        public string StickerPathToJson(params string[] values)
        {
            return JsonConvert.SerializeObject(values);

        }

        public void SaveImageAsSticker(Texture2D texture, string stickerName, int quality)
        {
            //Quality should be 90
            CallFunctionFromPlugin("SaveImageAsSticker", GetCurrentActivity(), GetSByteFromTexture(texture), stickerName, quality);;
        }

        public void SaveImageAsTray(Texture2D texture, string stickerName, int quality)
        {
            //Quality should be 90
            CallFunctionFromPlugin("SaveImageAsTray", GetCurrentActivity(), GetSByteFromTexture(texture), stickerName, quality);
        }

        private sbyte[] GetSByteFromTexture(Texture2D sourceTexture)
        {
            byte[] pixels = sourceTexture.EncodeToPNG();

            // Chuyển mảng byte sang mảng sbyte
            sbyte[] sPixels = new sbyte[pixels.Length];
            for (int i = 0; i < pixels.Length; i++)
            {
                sPixels[i] = (sbyte)pixels[i];
            }
            return sPixels;
        }
    }

}
