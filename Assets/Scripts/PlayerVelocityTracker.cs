using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVelocityTracker : MonoBehaviour
{
        [Header("Buffer length (frames)")]
        public int bufferSize = 10;

        private readonly Vector3[] posBuffer = new Vector3[32]; // 32 max, but we’ll use bufferSize
        private readonly float[] timeBuffer = new float[32];

        private int head;            // slot to overwrite next
        private int count;           // how many samples are valid

        public Vector3 SmoothedVelocity { get; private set; }

        void Awake()
        {
            head = 0;
            count = 0;
        }

        void FixedUpdate()
        {
            // 1. Store current sample
            posBuffer[head] = transform.position;
            timeBuffer[head] = Time.fixedTime;
            head = (head + 1) % bufferSize;
            count = Mathf.Min(count + 1, bufferSize);

            // 2. Use oldest valid sample to compute velocity
            if (count >= 2)
            {
                int tail = (head + bufferSize - 1) % bufferSize;        // newest sample index
                int oldest = (head + bufferSize - count) % bufferSize;  // oldest sample index

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
