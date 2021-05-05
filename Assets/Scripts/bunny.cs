using UnityEngine;

public class bunny : MonoBehaviour {

	public float fitnessScore = 1;
	public float temperature;
	public Vector3 size;
	public Color color = new Color(1,1,1);
	private Material[] materials;

	void Awake()
	{
		Renderer[] rends = GetComponentsInChildren<Renderer> ();
		materials = new Material[rends.Length];
		for (int i = 0; i < rends.Length; i++) 
		{
			materials [i] = rends [i].material;
		}
	}

	public void SetColor(Color color)
	{
		this.color = color;
		foreach (Material mat in materials) 
		{
			mat.color = color;
		}
	}

	public void SetSize(Vector3 Size)
	{
		size = Size;

		transform.localScale = size;
	}

	public void SetTemp(float temp)
	{
		temperature = temp;
	}
}
