using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShockWaveMaterial : MonoBehaviour
{
    [SerializeField] private float shockwaveTime = 0.75f;

    private Coroutine shockwaveCoroutine;
    public Material material;
    private static int waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");

    void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            CallShockwave();
        }
    }

    public void CallShockwave()
    {
        shockwaveCoroutine = StartCoroutine(ShockWaveAction(-0.1f, 0.5f));
    }

    private IEnumerator ShockWaveAction(float startPos, float endPos)
    {
        material.SetFloat(waveDistanceFromCenter, startPos);

        float lerpedAmount = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < shockwaveTime)
        {
            elapsedTime += Time.deltaTime;
            lerpedAmount = Mathf.Lerp(startPos, endPos, (elapsedTime / shockwaveTime));
            material.SetFloat(waveDistanceFromCenter, lerpedAmount);

            yield return null;
        }
    }
}
