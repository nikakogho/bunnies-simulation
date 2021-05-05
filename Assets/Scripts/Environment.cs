using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Environment : MonoBehaviour {

	public float colorTransitionTime = 5, sizeTransitionTime = 5, tempTransitionTime = 5;
	public float temperature;
	private Material mat;

	void Start()
	{
		mat = GetComponent<Renderer> ().material;
		StartCoroutine (Cycle ());
	}

	IEnumerator Cycle()
	{
		Vector3 previousColor = new Vector3 (mat.color.r, mat.color.g, mat.color.b);

		Vector3 currentColor = previousColor;

		Vector3 previousSize = transform.localScale;

		Vector3 currentSize = previousSize;

		float previousTemp = temperature;

		float currentTemp = temperature;

		while (true) 
		{
			Vector3 newColor = new Vector3 (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f));

			Vector3 deltaColor = (newColor - previousColor) * (1f / colorTransitionTime);

			Vector3 newSize = new Vector3 (Random.Range (Population.minSize, Population.maxSize), Random.Range (Population.minSize, Population.maxSize), Random.Range (Population.minSize, Population.maxSize));

			Vector3 deltaSize = (newSize - previousSize) * (1f / sizeTransitionTime);

			float newTemp = Random.Range (Population.minTemperature, Population.maxTemperature);

			float deltaTemp = (newTemp - previousTemp) * (1f / tempTransitionTime);

			while((newColor - currentColor).magnitude > 0.1f)
			{
				currentColor += deltaColor * Time.deltaTime;
				mat.color = new Color (currentColor.x, currentColor.y, currentColor.z);

				yield return null;
			}

			while((newSize - currentSize).magnitude > 0.1f)
			{
				currentSize += deltaSize * Time.deltaTime;

				transform.localScale = currentSize;

				yield return null;
			}

			while (Mathf.Abs (newTemp - currentTemp) > 0.1f) 
			{
				currentTemp += deltaTemp * Time.deltaTime;

				temperature = currentTemp;

				yield return null;
			}

			previousColor = newColor;
			previousSize = newSize;
			previousTemp = newTemp;
		}
	}
}
