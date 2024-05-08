using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using WASticker;

public class MainDemoController : MonoBehaviour
{
    [SerializeField]
    private Text textStatus;

    [SerializeField]
    private WAStickerModule wAStickerModule;

    void Start()
    {
        
    }

    // Update is called once per frame
#if UNITY_ANDROID && !UNITY_EDITOR
    void Update()
    {
        textStatus.text = $"Library Status: {wAStickerModule.GetLibraryStatus()}";
    }
#endif
    string stickerIdentifier = $"5";
    string stickerPackName = "StickerPack 5";
    string[] stickerPaths;
    string trayFile = "file_sticker_tray.png";
    public void ClickSetSticker()
    {
        wAStickerModule.ApplyStickerToWhatsapp(stickerIdentifier, stickerPackName);
        wAStickerModule.ToastMessage("Set");
    }

    public void ClickGenerateImage()
    {
        string stickerFileName = "file_sticker.webp";
        GenerateStickerOrTray("sticker", stickerFileName, 0);
        GenerateStickerOrTray("sticker_tray", trayFile, 1);
        stickerPaths = new string[] { stickerFileName, stickerFileName, stickerFileName };
        wAStickerModule.SetStickerInformation(stickerIdentifier, stickerPackName, "TienUU", "1", trayIconPath: trayFile, stickerPaths);
        wAStickerModule.ToastMessage("Generated");
    }

    private IEnumerator Wait(string stickerIdentifier, string stickerPackName)
    {
        yield return new WaitForSeconds(2);
        wAStickerModule.ApplyStickerToWhatsapp(stickerIdentifier, stickerPackName);
        wAStickerModule.ToastMessage("Set");
    }

    public void GenerateStickerOrTray(string resourcesPath, string fileName, int trayOrSticker) // 1 is tray, 0 is sticker
    {
        
        // Đường dẫn đến thư mục internal của ứng dụng
        string internalPath = Application.persistentDataPath;

        // Đọc nội dung của file từ thư mục Resources
        byte[] fileBytes = null;

        Texture2D texture = Resources.Load<Texture2D>(resourcesPath);
        Texture2D decompressTexture = DeCompress(texture);
        if (trayOrSticker == 0)
        {
            wAStickerModule.SaveImageAsSticker(decompressTexture, fileName, 90);
        }
        else
        {
            wAStickerModule.SaveImageAsTray(decompressTexture, fileName, 90);
        }
    }

    public Texture2D DeCompress(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
}
