using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogVideoFileLoad : MonoBehaviour
{
    public MediaPlayer mediaPlayer;

    public void OpenVideoFile()
    {
        string title = "请选择打开的文件：";
        //string msg = string.Empty;

        string path = FileDialogForWindows.FileDialog(title, 3, "");

        if (!string.IsNullOrEmpty(path))
        {
            InteractionUILayerCtr.instance.ImageCanvas.SetActive(false);
            InteractionUILayerCtr.instance.VideoCanvas.SetActive(true);
            Debug.Log("指定的文件路径为: " + path);
            mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, path, true);

        }
        else
        {
            Debug.Log("用户未作选择！");
        }
    }
}
