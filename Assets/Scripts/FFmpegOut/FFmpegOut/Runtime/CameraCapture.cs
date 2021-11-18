// FFmpegOut - FFmpeg video encoding plugin for Unity
// https://github.com/keijiro/KlakNDI

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace FFmpegOut
{


    [AddComponentMenu("FFmpegOut/Camera Capture")]
    public sealed class CameraCapture : MonoBehaviour
    {
        #region Public properties

        public Text _urlDebug;

        [SerializeField] int _width = 1920;

        public int width {
            get { return _width; }
            set { _width = value; }
        }

        [SerializeField] int _height = 1080;

        public int height {
            get { return _height; }
            set { _height = value; }
        }

        [SerializeField] FFmpegPreset _preset;

        public FFmpegPreset preset {
            get { return _preset; }
            set { _preset = value; }
        }

        [SerializeField] float _frameRate = 60;

        public float frameRate {
            get { return _frameRate; }
            set { _frameRate = value; }
        }

        #endregion

        #region Private members

        FFmpegSession _session;
        RenderTexture _tempRT;
        GameObject _blitter;

        RenderTextureFormat GetTargetFormat(Camera camera)
        {
            return camera.allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
        }

        int GetAntiAliasingLevel(Camera camera)
        {
            return camera.allowMSAA ? QualitySettings.antiAliasing : 1;
        }

        #endregion

        #region Time-keeping variables

        int _frameCount;
        float _startTime;
        int _frameDropCount;

        float FrameTime {
            get { return _startTime + (_frameCount - 0.5f) / _frameRate; }
        }

        void WarnFrameDrop()
        {
            if (++_frameDropCount != 10) return;

            Debug.LogWarning(
                "Significant frame droppping was detected. This may introduce " +
                "time instability into output video. Decreasing the recording " +
                "frame rate is recommended."
            );
        }

        #endregion

        #region MonoBehaviour implementation

        Camera mcamera;

        public bool isCapture;

        private void Awake()
        {

            mcamera = GetComponent<Camera>();
        }

        void OnValidate()
        {
            _width = Mathf.Max(8, _width);
            _height = Mathf.Max(8, _height);
        }

        void OnDisable()
        {
            if (_session != null)
            {
                // Close and dispose the FFmpeg session.
                _session.Close();
                _session.Dispose();
                _session = null;
            }

            if (_tempRT != null)
            {
                // Dispose the frame texture.
                GetComponent<Camera>().targetTexture = null;
                Destroy(_tempRT);
                _tempRT = null;
            }

            if (_blitter != null)
            {
                // Destroy the blitter game object.
                Destroy(_blitter);
                _blitter = null;
            }
        }

        IEnumerator StartFFsync()
        {
            // Sync with FFmpeg pipe thread at the end of every frame.
            for (var eof = new WaitForEndOfFrame();;)
            {
                yield return eof;
                _session?.CompletePushFrames();
            }
        }



        public void CopyFile(string srcFile, string destDir)
        {
            DirectoryInfo destDirectory = new DirectoryInfo(destDir);
            string fileName = Path.GetFileName(srcFile);
            if (!File.Exists(srcFile))
            {
                return;
            }

            if (!destDirectory.Exists)
            {
                destDirectory.Create();
            }

            File.Copy(srcFile, destDirectory.FullName + @"\" + fileName,true);

            Debug.Log("CopyFile,successed");
        }





        private string getFFmpegOutPath()
        {
            string[] PATH = Application.dataPath.Split('/');

            string TEMP = "";
            for (int i = 0; i < PATH.Length - 1; i++)
            {
                TEMP += PATH[i] + "/";
            }
            return TEMP;
        }



        public void StopCapture()
        {
            isCapture = false;


            if (_session != null)
            {
                // Close and dispose the FFmpeg session.
                _session.Close();
                _session.Dispose();
                _session = null;
            }

            if (_tempRT != null)
            {
                // Dispose the frame texture.
                GetComponent<Camera>().targetTexture = null;
                Destroy(_tempRT);
                _tempRT = null;
            }

            if (_blitter != null)
            {
                // Destroy the blitter game object.
                Destroy(_blitter);
                _blitter = null;
            }

            _urlDebug.text = getFFmpegOutPath() + FFmpegSession.GetCurrentFileName();

            CopyFile(_urlDebug.text, Application.streamingAssetsPath);

            
        }

        public void StartCapture() {
            // Lazy initialization
            if (_session == null)
            {
                // Give a newly created temporary render texture to the camera
                // if it's set to render to a screen. Also create a blitter
                // object to keep frames presented on the screen.
                if (mcamera.targetTexture == null)
                {
                    _tempRT = new RenderTexture(_width, _height, 24, GetTargetFormat(mcamera));
                    _tempRT.antiAliasing = GetAntiAliasingLevel(mcamera);
                    mcamera.targetTexture = _tempRT;
                    _blitter = Blitter.CreateInstance(mcamera);
                }

                // Start an FFmpeg session.
                _session = FFmpegSession.Create(
                    gameObject.name,
                    mcamera.targetTexture.width,
                    mcamera.targetTexture.height,
                    _frameRate, preset
                );

                _startTime = Time.time;
                _frameCount = 0;
                _frameDropCount = 0;
            }

            StartCoroutine(StartFFsync());



            StartCoroutine(get10SECvideo());
        }


        IEnumerator get10SECvideo()
        {
            yield return new WaitForSeconds(1);
                            isCapture = true;
            //yield return new WaitForSeconds(10);
            //StopCapture();

        }

        void Update()
        {
           if (isCapture)
            {
                var gap = Time.time - FrameTime;
                var delta = 1 / _frameRate;

                if (gap < 0)
                {
                    // Update without frame data.
                    _session.PushFrame(null);
                }
                else if (gap < delta)
                {
                    // Single-frame behind from the current time:
                    // Push the current frame to FFmpeg.
                    _session.PushFrame(mcamera.targetTexture);
                    _frameCount++;
                }
                else if (gap < delta * 2)
                {
                    // Two-frame behind from the current time:
                    // Push the current frame twice to FFmpeg. Actually this is not
                    // an efficient way to catch up. We should think about
                    // implementing frame duplication in a more proper way. #fixme
                    _session.PushFrame(mcamera.targetTexture);
                    _session.PushFrame(mcamera.targetTexture);
                    _frameCount += 2;
                }
                else
                {
                    // Show a warning message about the situation.
                    WarnFrameDrop();

                    // Push the current frame to FFmpeg.
                    _session.PushFrame(mcamera.targetTexture);

                    // Compensate the time delay.
                    _frameCount += Mathf.FloorToInt(gap * _frameRate);
                }
            }
        }
        #endregion
    }
}
