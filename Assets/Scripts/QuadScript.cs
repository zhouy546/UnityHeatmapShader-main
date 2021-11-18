using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class QuadScript : MonoBehaviour
{
    public Camera UserLookCamera;
  Material mMaterial;
  MeshRenderer mMeshRenderer;

  float[] mPoints;
  int mHitCount;

  float mDelay=0.01f;

    private OutPutTxt output;


  void Start()
  {
    EventCenter.AddListener(EventDefine.onVideoStartCapture, async() => { await setStartCapture(); });

    EventCenter.AddListener(EventDefine.onVideoStopCapture, () => { ValueSheet.isCaptureing = false; output.LOOP = false; });

    EventCenter.AddListener(EventDefine.onVideoStopCapture,removePoint);

    EventCenter.AddListener(EventDefine.TurnOffUI, CullMaskHeatmapOff);

    EventCenter.AddListener(EventDefine.TurnOnUI, CullMaskHeatmapOn);

    mMeshRenderer = GetComponent<MeshRenderer>();

    mMaterial = mMeshRenderer.material;

    mPoints = new float[200 * 3]; //60 point 

        output = new OutPutTxt();

  }

    private async Task setStartCapture()
    {
        await Task.Delay(1000);

        ValueSheet.currentTime = 0;

        ValueSheet.isCaptureing = true;

        output.StartThread();
    }

    private void OnApplicationQuit()
    {
        if (output.t != null)
        {
            output.t.Abort();
        }
    }

    private void CullMaskHeatmapOn()
    {
        UserLookCamera.cullingMask = (1 << LayerMask.NameToLayer("UI")) | (1 << LayerMask.NameToLayer("TestPhotoLayer")) | (1 << LayerMask.NameToLayer("HeatMapUILayer"));
    }

    private void CullMaskHeatmapOff()
    {
        UserLookCamera.cullingMask = (1 << LayerMask.NameToLayer("UI")) | (1 << LayerMask.NameToLayer("TestPhotoLayer"));
    }

    void FixedUpdate()
    {

        mDelay -= Time.deltaTime;
        if (mDelay <= 0)
        {

            if (ValueSheet.isCaptureing)
            {
                ValueSheet.currentTime += Time.deltaTime;

                RaycastHit hit;

                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                bool hitit = Physics.Raycast(ray, out hit, 11f, LayerMask.GetMask("HeatMapMeshLayer"));

                if (hitit)
                {
                    addHitPoint(hit.textureCoord.x * 2 - 1, hit.textureCoord.y * 2 - 1);
                }

                string X = Mathf.RoundToInt(Input.mousePosition.x).ToString();
                string Y = Mathf.RoundToInt(Input.mousePosition.y).ToString();

                string S = "时间："+ValueSheet.currentTime.ToString("#0.00")+"    " + "X坐标："+ X+ "    "+ "Y坐标："+Y;

                ValueSheet.output.Enqueue(S);
            }

            mDelay = 0.01f;
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //  foreach(ContactPoint cp in collision.contacts)
    //  {
    //    Debug.Log("Contact with object " + cp.otherCollider.gameObject.name);

    //    Vector3 StartOfRay = cp.point - cp.normal;
    //    Vector3 RayDir = cp.normal;

    //    Ray ray = new Ray(StartOfRay, RayDir);
    //    RaycastHit hit;

    //    bool hitit = Physics.Raycast(ray, out hit, 10f, LayerMask.GetMask("HeatMapLayer"));

    //    if (hitit)
    //    {
    //      Debug.Log("Hit Object " + hit.collider.gameObject.name);
    //      Debug.Log("Hit Texture coordinates = " + hit.textureCoord.x + "," + hit.textureCoord.y);
    //      addHitPoint(hit.textureCoord.x*4-2, hit.textureCoord.y*4-2);
    //    }
    //    Destroy(cp.otherCollider.gameObject);
    //  }
    //}

    public void removePoint()
    {
        for (int i = 0; i < 200; i++)
        {
            addHitPoint(2000, 2000);
        }
    }

    public void addHitPoint(float xp,float yp)
  {
    mPoints[mHitCount * 3] = xp;
    mPoints[mHitCount * 3 + 1] = yp;
    mPoints[mHitCount * 3 + 2] = Random.Range(1f, 3f);

    mHitCount++;
    mHitCount %= 200;

    mMaterial.SetFloatArray("_Hits", mPoints);
    mMaterial.SetInt("_HitCount", mHitCount);

  }

}
