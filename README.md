# UnityWhatsappSticker
# Introduction

Adding sticker to whatsapp from unity application

## Installation

### Import the library
Got the wasticker-release.aar from package or clone project and add it to folder ../Plugins/Android/

### Declare Sticker Content Provider

Check Custom Android Main Manifest in Build Settings/Player Settings/ Publishing Settings/Build and add this into application tag <application></application>

![alt text](https://github.com/ngtien137/UnityWhatsappSticker/blob/main/tutorial_images/tut1.png)

```gradle
<manifest
    xmlns:android="http://schemas.android.com/apk/res/android"
    package="com.unity3d.player"
    xmlns:tools="http://schemas.android.com/tools">
    <application>
      <activity>
            .......
      </activity>
      .....
      <provider
                 android:name="com.lemon.wasticker.StickerContentProvider"
                 android:authorities="{applicationId}.StickerContentProvider"
                 android:enabled="true"
                 android:exported="true"
                 android:readPermission="com.whatsapp.sticker.READ" />
    </application>
</manifest>
```

You can see variable {applicationId} above. It means the application PackageName in unity. You can find and set it in Other settings:

![alt text]([http://url/to/img.png](https://github.com/ngtien137/UnityWhatsappSticker/blob/main/tutorial_images/tut2.png))

Example: In demo project, the applicationId (or packageName) is com.luza.whatsapp_sticker:

```gradle
      <provider
                 android:name="com.lemon.wasticker.StickerContentProvider"
                 android:authorities="com.luza.whatsapp_sticker.StickerContentProvider"
                 android:enabled="true"
                 android:exported="true"
                 android:readPermission="com.whatsapp.sticker.READ" />
```

## Guide

### Adding Module To Scene
Create an empty game object and attach WAStickerModule.cs to it. Then you can use that module for creating custom sticker

### Working with sticker pack
* Talk about a Sticker Pack. This is an sticker pack:
![alt text]([http://url/to/img.png](https://github.com/ngtien137/UnityWhatsappSticker/blob/main/tutorial_images/sticker_pack.png))
  Identifier: May be know as id of sticker, a string but you need set int or long to it. If you add an sticker pack with the same identifier, whatsapp will ignore you adding sticker pack. So you need use another identifier (I don't support update sticker pack)

* For creating custom sticker pack, you need at least two **PNG** images. One of them is tray icon and the others are Stickers
* Tray icon size is max 50kb, in **96x96 pixel**, in .png format
* Sticker size is max 100kb, in **512x512 pixels**, in .png format (for my library converting png to webp)

* If you have valid images, then you can use this function to save texture to temp storage for adding stickers in the futures
* Use function **SaveImageAsSticker(Texture2D texture, string stickerName, int quality)** and **SaveImageAsTray(Texture2D texture, string stickerName, int quality)** to create 

```java
public class MainDemoController : MonoBehaviour
{
    [SerializeField]
    private WAStickerModule wAStickerModule;

    public void ClickGenerateImage()
    {
        string stickerFileName = "file_sticker.webp";
        GenerateStickerOrTray("sticker", stickerFileName, 0);
        GenerateStickerOrTray("sticker_tray", trayFile, 1);
        stickerPaths = new string[] { stickerFileName, stickerFileName, stickerFileName };
        wAStickerModule.SetStickerInformation(stickerIdentifier, stickerPackName, "TienUU", "1", trayIconPath: trayFile, stickerPaths);
        wAStickerModule.ToastMessage("Generated");
    }

    public void GenerateStickerOrTray(string resourcesPath, string fileName, int trayOrSticker) // 1 is tray, 0 is sticker
    {
        
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

    //Some texture2D need be decompressed before using
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
```

* Above is code for save temp stickers before adding to whatsapp. SaveImageAsSticker has a paramater quality which helps you adjust size of image (should be use at 90 if you think your image will work)

* Sticker pack need at least three stickers for available. But here you can use a trick. Add your sticker and two path of sticker which are not exists. So you have a sticker pack with only one sticker

* Example:

```java
stickerPaths = new string[] { stickerFileName, "lasdjflasdjflajsdf.png", "rqweoruqwer.png" };

//Two file name above still need end with .png
```

* You need add all sticker and information of sticker pack after you create them by function **SetStickerInformation(string identifier, string stickerPackName, string publisher, string imageDataVersion, string trayIconPath, params string[] stickersPath)**:

```java
  public void SetStickerInformation(string identifier, string stickerPackName, string publisher, string imageDataVersion, string trayIconPath, params string[] stickersPath)
  //image data version means nothing in my code, so set it "1" or what you want
```

### Ready for adding stickers
Now you are ready, call this function: ****
```java
public bool ApplyStickerToWhatsapp(string identifier, string stickerPackName)

//Example

public void ClickSetSticker()
{
    bool success = wAStickerModule.ApplyStickerToWhatsapp(stickerIdentifier, stickerPackName);
    wAStickerModule.ToastMessage("Set");
}

```
If it show you a dialog confirm, you are success. Good luck!
