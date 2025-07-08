using System;
using UnityEngine;

namespace Start
{
    [Serializable]
    public class SerializedVector3
    {
        public float x;
        public float y;
        public float z;
        public SerializedVector3()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public SerializedVector3(SerializedVector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public SerializedVector3(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public SerializedVector3(float xx, float yy, float zz)
        {
            x = xx;
            y = yy;
            z = zz;
        }
        public static implicit operator SerializedVector3(Vector3 v)
        {
            return new SerializedVector3(v.x, v.y, v.z);
        }

        public static implicit operator Vector3(SerializedVector3 v)
        {
            if (v == null)
            {
                return Vector3.zero;
            }
            return new Vector3(v.x, v.y, v.z);
        }
    }

    [Serializable]
    public class SerializedTransform
    {
        public SerializedVector3 position = new SerializedVector3();
        public SerializedVector3 angle = new SerializedVector3();
        public SerializedVector3 scale = new SerializedVector3();

        public static implicit operator SerializedTransform(Transform transform)
        {
            SerializedTransform pTransform = new SerializedTransform();
            pTransform.position = transform.position;

            pTransform.angle = transform.eulerAngles;
            pTransform.scale = transform.localScale;
            return pTransform;
        }
    }

    [Serializable]
    public class SerializedBoxCollider
    {
        public bool isTrigger;
        public SerializedTransform transform;
        public SerializedVector3 center;
        public SerializedVector3 size;

        public static implicit operator SerializedBoxCollider(BoxCollider boxCollider)
        {
            SerializedBoxCollider config = new SerializedBoxCollider();
            config.transform = (SerializedTransform)boxCollider.gameObject.transform;
            config.center = boxCollider.center;
            config.size = boxCollider.size;
            config.isTrigger = boxCollider.isTrigger;
            return config;
        }
    }

    [Serializable]
    public class SerializedSphereCollider
    {
        public bool isTrigger;
        public SerializedTransform transform;
        public SerializedVector3 center;
        public float radius;

        public static implicit operator SerializedSphereCollider(SphereCollider sphereCollider)
        {
            SerializedSphereCollider config = new SerializedSphereCollider();
            config.transform = (SerializedTransform)sphereCollider.gameObject.transform;
            config.center = (SerializedVector3)sphereCollider.center;
            config.radius = sphereCollider.radius;
            config.isTrigger = sphereCollider.isTrigger;
            return config;
        }
    }
}