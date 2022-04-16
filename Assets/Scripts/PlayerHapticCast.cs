using Bhaptics.Tact.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHapticCast : MonoBehaviour
{
    [Range(1, 32)]
    public int numSensors = 4;
    [Range(1, 32)]
    public int numLevels = 4;
    [Range(1, 50)]
    public int sensorLength = 10;

    [Range(0.01f, 0.8f)]
    public float intensityMultiplier = 0.2f;

    public float sensorHeightOffset = 0.5f;

    RaycastHit[,] sensorArray;

    float[] sensorHeights;
    float[] sensorAngles;

    float middleHeight;

    float maxMinHeight = 0.5f;

    private int curNumSensors;
    private int curNumLevels;
    private float curHeightOffset;

    public VestHapticClip clip;

    // Start is called before the first frame update
    void Start()
    {
        curNumLevels = numLevels;
        curNumSensors = numSensors;
        curHeightOffset = sensorHeightOffset;
        middleHeight = gameObject.transform.position.y + sensorHeightOffset;
        CreateSensorArray(numLevels, numSensors);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (curNumSensors != numSensors || curNumLevels != numLevels || curHeightOffset != sensorHeightOffset)
        {
            middleHeight = gameObject.transform.position.y + sensorHeightOffset;
            curNumLevels = numLevels;
            curNumSensors = numSensors;
            CreateSensorArray(curNumLevels, curNumSensors);
        }
        CastRays();
    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    void CreateSensorArray(int levels, int sensors)
    {
        sensorArray = new RaycastHit[levels, sensors];
        sensorHeights = CalculateSensorHeights(levels);
        sensorAngles = CalculateSensorAngles(sensors);
    }

    void CastRays()
    {
        //Need this so rays only hit walls;
        int layerMask = 1 << 3;
        for (int i = 0; i < curNumLevels; i++)
        {
            for(int j = 0; j < curNumSensors; j++)
            {

                Quaternion rot = Quaternion.AngleAxis(sensorAngles[j], Vector3.up);
                Vector3 lDirection = rot * Vector3.forward;
                lDirection.Normalize();
                //lDirection = lDirection * sensorLength;

                Vector3 rayPosition = new Vector3(transform.position.x, middleHeight + sensorHeights[i], transform.position.z);

                if (Physics.Raycast(rayPosition, transform.TransformDirection(lDirection), out sensorArray[i,j], sensorLength, layerMask))
                {
                    Debug.DrawRay(rayPosition, transform.TransformDirection(lDirection) * sensorArray[i, j].distance, Color.green);
                    DoHaptics(sensorAngles[j], sensorHeights[i], sensorArray[i,j].distance);
                }
                else
                {
                    Debug.DrawRay(rayPosition, transform.TransformDirection(lDirection * sensorLength), Color.red);
                }
            }
        }
    }

    void DoHaptics(float angle, float offset, float distance)
    {
        //Debug.Log("Haptics: Angle: " + angle + " Offset: " + offset + " Distance: " + distance);
        if (clip != null)
        {
            clip.Play(((sensorLength - distance) + intensityMultiplier), 1, -angle, offset);
        }
    }

    
    private float[] CalculateSensorAngles(int sensors)
    {
        Debug.Log("New angles");
        float[] newSensorAngles = new float[sensors];
        float totalAngleDegree = 360f;
        float step = totalAngleDegree / (float)sensors;
        if(sensors == 1)
        {
            newSensorAngles[0] = 0.0f;
        } else {
            for (int i = 0; i <= Mathf.Floor(sensors / 2.0f) - 1; i++)
            {
                newSensorAngles[i] = step * (i + 1);
                newSensorAngles[(sensors - 1) - i] = -(step * (i + 1));
            }

            if(sensors % 2 != 0)
            {
                newSensorAngles[sensors / 2] = 0.0f;
            }
        }
        return newSensorAngles;
    }

    private float[] CalculateSensorHeights(int levels)
    {
        Debug.Log("New sensor heights");
        float[] newSensorHeights = new float[levels];
        float step = maxMinHeight / (float)levels;

        if (levels == 1)
        {
            newSensorHeights[0] = 0.0f;
        } else
        {
            for (int i = 0; i <= Mathf.Floor(levels / 2.0f) - 1; i++)
            {
                newSensorHeights[i] = step * (i + 1);
                newSensorHeights[(levels - 1) - i] = -(step * (i + 1));
            }

            if (levels % 2 != 0)
            {
                newSensorHeights[levels / 2] = 0.0f;
            }
        }
        return newSensorHeights;
    }
}
