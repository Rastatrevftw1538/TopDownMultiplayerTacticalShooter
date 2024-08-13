using UnityEngine;
using System.Collections;
using Cinemachine;

public class CameraShake : Singleton<CameraShake>
{
	// Transform of the camera to shake. Grabs the gameObject's transform
	// if null.
	public Transform camTransform;

	// How long the object should shake for.
	public float shakeDuration = 0.8f;

	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.4f;
	public float decreaseFactor = 1.0f;

	Vector3 originalPos;

	public void CustomCameraShake(CinemachineImpulseSource source)
	{
        source.GenerateImpulseWithForce(shakeAmount);
	}

	void OnEnable()
	{
		//noiseSettings = cinemachine.AddExtension<NoiseSettings>();
		originalPos = camTransform.localPosition;
		StartCoroutine(QuickEnable());
	}

	IEnumerator QuickEnable()
    {
		yield return new WaitForSeconds(shakeDuration);
		this.enabled = false;
	}

	void Update()
	{
		if (shakeDuration > 0)
		{
			camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

			shakeDuration -= Time.deltaTime * decreaseFactor;
		}
		else
		{
			shakeDuration = 0f;
			camTransform.localPosition = originalPos;
		}
	}

	float tempAmt;
	float tempDur;
	public IEnumerator CustomCameraShake(float _shakeDuration, float _shakeAmount)
    {
        tempAmt = shakeAmount;
		tempDur = shakeDuration;

		shakeAmount = _shakeAmount;
		shakeDuration = _shakeDuration;

		Invoke(nameof(RevertValues), shakeDuration);
		yield return new WaitForSeconds(_shakeDuration);
    }

	private void RevertValues()
    {
		shakeAmount = tempAmt;
		shakeDuration = tempDur;
	}
}