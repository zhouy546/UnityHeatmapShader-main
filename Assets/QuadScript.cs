using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadScript : MonoBehaviour
{
  Material mMaterial;
  MeshRenderer mMeshRenderer;

  float[] mPoints;
  int mHitCount;

  float mDelay;


  void Start()
  {
    mDelay = 3;

    mMeshRenderer = GetComponent<MeshRenderer>();
    mMaterial = mMeshRenderer.material;

    mPoints = new float[200 * 3]; //60 point 

  }

  void FixedUpdate()
  {
        //mDelay -= Time.deltaTime;
        //if (mDelay <=0)
        //{
        //  GameObject go = Instantiate(Resources.Load<GameObject>("Projectile"));
        //  go.transform.position = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-3f, -1f));

        //  mDelay = 0.5f;
        //}
        //mDelay -= Time.deltaTime;
        //if (mDelay <= 0)
        //{
            RaycastHit hit;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            bool hitit = Physics.Raycast(ray, out hit, 11f, LayerMask.GetMask("HeatMapLayer"));

        Debug.Log(hitit);

            if (hitit)
            {
                //Debug.Log("Hit Object " + hit.collider.gameObject.name);
                //Debug.Log("Hit Texture coordinates = " + hit.textureCoord.x + "," + hit.textureCoord.y);
                addHitPoint(hit.textureCoord.x * 2 - 1, hit.textureCoord.y * 2 - 1);
            }
        //    mDelay = 0.5f;
        //}
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
