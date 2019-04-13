using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TMP_Benchmark : MonoBehaviour
{
    const int MAX_ITERATIONS = 1;
    bool _benchStarted = false;

    void Update()
    {
        if (Input.GetKey(KeyCode.A) && !_benchStarted)
        {
            StartBenchmark();
        }
    }

    
    void StartBenchmark()
    {
        _benchStarted = true;

        var s1 = Stopwatch.StartNew();
        for (int i = 0; i < MAX_ITERATIONS; i++)
        {
            float rng = Random.Range(1, 10);
            float o = rng / 2f;
        }
        s1.Stop();

        var s2 = Stopwatch.StartNew();
        for (int i = 0; i < MAX_ITERATIONS; i++)
        {
            float rng = Random.Range(1, 10);
            float o = rng * 0.5f;
        }
        s2.Stop();

        UnityEngine.Debug.Log(((double)(s1.Elapsed.TotalMilliseconds * 1000000) / MAX_ITERATIONS).ToString("0.00 ns"));
        UnityEngine.Debug.Log(((double)(s2.Elapsed.TotalMilliseconds * 1000000) / MAX_ITERATIONS).ToString("0.00 ns"));
    }
}
