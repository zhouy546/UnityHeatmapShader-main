using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class DialogTest : MonoBehaviour
{
    public Image BGimg;

    public void OpenImageFile()
    {
        string title = "请选择打开的文件：";
        //string msg = string.Empty;

        string path = FileDialogForWindows.FileDialog(title, 3, "");

        if (!string.IsNullOrEmpty(path))
        {
            InteractionUILayerCtr.instance.ImageCanvas.SetActive(true);
            InteractionUILayerCtr.instance.VideoCanvas.SetActive(false);
            Debug.Log("指定的文件路径为: " + path);
            LoadImg(path);

        }
        else
        {
            Debug.Log("用户未作选择！");
        }
    }

    async void LoadImg(string _imageUrl)
    {
        Texture2D _texture = await Utility.GetRemoteTexture(_imageUrl);
        BGimg.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0.5f, 0.5f));

    }

    public void SaveFile(string pathname)
    {
        string title = "请选择保存的位置：";
        //string msg = string.Empty;
        string savepath = FileDialogForWindows.SaveDialog(title, Path.Combine(Application.streamingAssetsPath, pathname));//假如你存rar文件。
        if (!string.IsNullOrEmpty(savepath))
        {
        //   Game_manager.Instance.msgText.text = "保存文件的路径为: " + savepath;

        }
        else
        {
          //  Game_manager.Instance.msgText.text = "用户取消保存！";
        }
    }


}