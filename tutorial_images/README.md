# UnityWhatsappSticker
# Introduction

Adding sticker to whatsapp from unity application

## Installation

### Import the library
Sao chép thư viện wasticker-release.aar từ unity package hoặc clone project và thêm nó vào thư mục ../Plugins/Android/

### Khai báo Sticker Content Provider

Bật tính năng Custom Android Main Manifest trong Build Settings/Player Settings/ Publishing Settings/Build và add dòng này vào trong tag application <application></application> trong file manifest:

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

Biến {applicationId} ở trên có nghĩa là PackageName trong unity. Có thể cài đặt và tìm thấy nó trong Other settings:

![alt text](https://github.com/ngtien137/UnityWhatsappSticker/blob/main/tutorial_images/tut2.png)

Ví dụ: Trong demo project, the applicationId (or packageName) chính là com.luza.whatsapp_sticker:

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
Tạo một game object trống và thêm WAStickerModule.cs vào nó

### Working with sticker pack
* Nói về một gói sticker (Sticker Pack). Dưới đây là một sticker pack:
![alt text](https://github.com/ngtien137/UnityWhatsappSticker/blob/main/tutorial_images/sticker_pack.png)
  Identifier: Hiểu đơn giản nó là id của sticker pack, mặc dù nó truyền vào là string nhưng hãy sử dụng int hoặc long cho nó. Nếu đã thêm một pack có id là "1" vào rồi, mà lại add một cái "1" vào nữa thì nó sẽ báo sticker này được add rồi và không cho add nữa, ở đây cần tính năng update, nhưng chịu, không hỗ trợ nhé. Hãy dùng một sticker identifier khác vào đấy.

* Để tạo một sticker pack thì cần ít nhất 2 ảnh **PNG**. Một trong số đó là tray icon, số còn lại là stickers. Đoạn này thì cũng không biết nó có nhất thiết cần png không, nhưng cứ căn thế trước để sửa dụng hàm Texture2D.EncodePNG gì đó cho an toàn, mục đích là để sử dụng hàm đó
* Tray icon có kích thước tối đa là 50kb, **96x96 pixel**, định dạng .png
* Sticker tĩnh có kích thước tối đa 100kb, **512x512 pixels**, định dạng .png (Thật ra để add vào whatsapp thì nó cần webp, nhưng thư viện cần là png và nó sẽ tự chuyển ảnh thành webp)

* Khi đã có đủ hình ảnh, sử dụng hàm **SaveImageAsSticker(Texture2D texture, string stickerName, int quality)** và **SaveImageAsTray(Texture2D texture, string stickerName, int quality)** để lưu tray icon và stickers vào một bộ nhớ tạm, tương lai sẽ cầm chính cái ảnh này để add vào stickers pack. stickerName ở đây sẽ bao gồm cả đuôi .webp còn nếu là tray thì sẽ bao gồm .png

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

* SaveImageAsSticker có một tham số quality từ 0-100, nó sẽ giúp điều chỉnh size của image (giống như ở trên nói kích cỡ tối đa của tray image là 50kb, sticker là 100). Biến này sẽ giúp điều chỉnh chất lượng hình ảnh sao cho phù hợp. Còn sao để phù hợp thì phải tự căn. Nếu nó báo lỗi không add được sticker pack thì cũng có thể là do ảnh đã bị vượt quá kích cỡ tối đa

* Sticker pack cần ít nhất 3 stickers để add. Nhưng ở đây có một trick. Thêm sticker và 2 cái tên không tồn tại vào, Vậy là sẽ tạo được một sticker pack với một sticker

* Example:

```java
stickerPaths = new string[] { stickerFileName, "lasdjflasdjflajsdf.png", "rqweoruqwer.png" };

//Two file name above still need end with .png or .webp
```

* Sau khi save xong thì sử dụng hàm này để khai báo sticker pack **SetStickerInformation(string identifier, string stickerPackName, string publisher, string imageDataVersion, string trayIconPath, params string[] stickersPath)**:

```java
  public void SetStickerInformation(string identifier, string stickerPackName, string publisher, string imageDataVersion, string trayIconPath, params string[] stickersPath)
  //image data version không có nghĩa lý gì cả, đặt là "1" cũng được, đáng lẽ là để update sticker pack, nhưng chưa code
```

### Ready for adding stickers
Xong rồi đấy, gọi nốt hàm này thôi: ****
```java
public bool ApplyStickerToWhatsapp(string identifier, string stickerPackName)

//Example

public void ClickSetSticker()
{
    bool success = wAStickerModule.ApplyStickerToWhatsapp(stickerIdentifier, stickerPackName);
    wAStickerModule.ToastMessage("Set");
}

```
* Lưu ý: Không được sử dụng hàm SetStickerInformation và ApplyStickerToWhatsapp trong cùng một thời điểm, ít nhất giữa 2 hàm này phải gọi 2 thời điểm khác nhau hoặc delay mấy giây để nó khởi tạo cái stickerpack
If it show you a dialog confirm, you are success. Good luck!
