using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVelocityTracker : MonoBehaviour
{
        [Header("Buffer length (frames)")]
        public int bufferSize = 10;

        private readonly Vector3[] posBuffer = new Vector3[32]; 
        private readonly float[] timeBuffer = new float[32];

        private int head;            
        private int count;          

        public Vector3 SmoothedVelocity { get; private set; }

        void Awake()
        {
            head = 0;
            count = 0;
        }

        void FixedUpdate()
        {
            
            posBuffer[head] = transform.position;
            timeBuffer[head] = Time.fixedTime;
            head = (head + 1) % bufferSize;
            count = Mathf.Min(count + 1, bufferSize);

            
            if (count >= 2)
            {
                int tail = (head + bufferSize - 1) % bufferSize;        
                int oldest = (head + bufferSize - count) % bufferSize;  

                Vector3 dp = posBuffer[tail] - posBuffer[oldest];
                float dt = timeBuffer[tail] - timeBuffer[oldest];
                SmoothedVelocity = (dt > 1e-4f) ? dp / dt : Vector3.zero;
            }
            else
            {
                SmoothedVelocity = Vector3.zero;
            }
        }
    }
