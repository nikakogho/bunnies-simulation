using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Population : MonoBehaviour {

	public float MinSize = 0.5f, MaxSize = 5, MinTemperature = 5, MaxTemperature = 15;
	public static float minSize, maxSize, minTemperature, maxTemperature;
	public Text generationText, tempText;
	public float evolveTime = 5;
	public int populationSize = 100;
	private Environment eScript;
	private  int Generation = 0;
	public GameObject environment;
	public GameObject Bunny;

	protected List<bunny> population = new List<bunny>();

	void Awake()
	{
		eScript = environment.GetComponent<Environment> ();
		minSize = MinSize;
		maxSize = MaxSize;
		minTemperature = MinTemperature;
		maxTemperature = MaxTemperature;
	}

	void Start()
	{
		Bounds boundaries = environment.GetComponent<Renderer> ().bounds;

		for (int i = 0; i < populationSize; i++) 
		{
			bunny rabbit = CreateBunny (boundaries);
			population.Add (rabbit);
		}

		StartCoroutine (EvaluationLoop ());
	}

	public bunny CreateBunny(Bounds bounds)
	{
		Vector3 randomPosition = new Vector3 (Random.Range (-0.5f, 0.5f) * bounds.size.x, Random.Range (-0.5f, 0.5f) * bounds.size.y, Random.Range (-0.5f, 0.5f) * bounds.size.z);

		Vector3 worldPosition = environment.transform.position + randomPosition;

		GameObject temp = Instantiate (Bunny, worldPosition, Quaternion.identity);
		bunny rabbit = temp.GetComponent<bunny> ();

		AssignRandomColor (temp);
		AssignRandomSize (temp);
		AssignRandomTemperature (temp);

		return rabbit;
	}
		
	public void AssignRandomColor(GameObject rabbit)
	{
		rabbit.GetComponent<bunny>().SetColor(new Color( Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
	}

	public void AssignRandomSize(GameObject rabbit)
	{
		rabbit.GetComponent<bunny>().SetSize(new Vector3( Random.Range(minSize, maxSize), Random.Range(minSize, maxSize), Random.Range(minSize, maxSize)));
	}

	public void AssignRandomTemperature(GameObject rabbit)
	{
		rabbit.GetComponent<bunny> ().SetTemp (Random.Range (minTemperature, maxTemperature));
	}

	float EvaluateFitness(Color environment, Color rabbit)
	{
		float Fitness = (new Vector3 (environment.r, environment.g, environment.b) - new Vector3 (rabbit.r, rabbit.g, rabbit.b)).magnitude;

		return Fitness;
	}

	float EvaluateSize(Vector3 environment, Vector3 rabbit)
	{
		float Fitness = (environment - rabbit).magnitude;

		return Fitness;
	}

	float EvaluateTemperature(float temp)
	{
		float Fitness = Mathf.Abs (environment.GetComponent<Environment> ().temperature - temp);

		return Fitness;
	}

	void EvaluatePopulation()
	{
		Color environmentColor = environment.GetComponent<Renderer> ().material.color;
		//Fitness
		for (int i = 0; i < population.Count; i++) 
		{
			float colorFitness = EvaluateFitness (environmentColor, population[i].GetComponentInChildren<Renderer>().material.color);
			float sizeFitness = EvaluateSize (environment.transform.localScale, population [i].size);
			float tempFitness = EvaluateTemperature (population [i].temperature);

			float fitness = colorFitness + sizeFitness + tempFitness;

			population [i].fitnessScore = fitness;
		}

		//sort
		population.Sort
		(
			delegate(bunny Bunny1, bunny Bunny2) 
			{
				if(Bunny1.fitnessScore > Bunny2.fitnessScore)return 1;
				else if(Bunny1.fitnessScore == Bunny2.fitnessScore)return 0;
				else return -1;
			}
		);

		//kill
		int halfwayMark = (int)population.Count / 2;

		if (halfwayMark % 2 != 0) 
		{
			halfwayMark++;
		}

		for (int i = halfwayMark; i < population.Count; i++) 
		{
			Destroy(population[i].gameObject);
			population [i] = null;
		}

		population.RemoveRange (halfwayMark, population.Count - halfwayMark);

		//breed
		Breed();
	}

	void Breed()
	{
		//new list

		List<bunny> tempList = new List<bunny> ();

		for (int i = 1; i < population.Count; i += 2) 
		{
			int index1 = i - 1;
			int index2 = i;

			float howGenesAreSplit = Random.Range (0f, 1f);

			Bounds bounds = environment.GetComponent<Renderer> ().bounds;

			bunny child1 = CreateBunny (bounds);
			bunny child2 = CreateBunny (bounds);

			tempList.Add (child1);
			tempList.Add (child2);

			if (howGenesAreSplit <= 0.16f) 
			{
				Color tempColor = new Color (population [index1].color.r, population [index1].color.g, population [index2].color.b);

				tempColor = mutatedColor (tempColor);

				child1.SetColor (tempColor);

				tempColor = new Color (population [index2].color.r, population [index2].color.g, population [index1].color.b);

				tempColor = mutatedColor (tempColor);

				child2.SetColor (tempColor);

				Vector3 tempSize = new Vector3 (population [index1].transform.localScale.x, population [index1].transform.localScale.y, population [index2].transform.localScale.z);

				tempSize = mutatedSize (tempSize);

				child1.SetSize (tempSize);

				tempSize = new Vector3 (population [index2].transform.localScale.x, population [index2].transform.localScale.y, population [index1].transform.localScale.z);

				tempSize = mutatedSize (tempSize);

				child2.SetSize (tempSize);
			}
			else if (howGenesAreSplit <= 0.32f) 
			{
				Color tempColor = new Color (population [index1].color.r, population [index1].color.g, population [index1].color.b);

				tempColor = mutatedColor (tempColor);

				child1.SetColor (tempColor);

				tempColor = new Color (population [index2].color.r, population [index2].color.g, population [index2].color.b);

				tempColor = mutatedColor (tempColor);

				child1.SetColor (tempColor);

				Vector3 tempSize = new Vector3 (population [index1].transform.localScale.x, population [index1].transform.localScale.y, population [index1].transform.localScale.z);

				tempSize = mutatedSize (tempSize);

				child1.SetSize (tempSize);

				tempSize = new Vector3 (population [index2].transform.localScale.x, population [index2].transform.localScale.y, population [index2].transform.localScale.z);

				tempSize = mutatedSize (tempSize);

				child2.SetSize (tempSize);
			}
			else if (howGenesAreSplit <= 0.48f) 
			{
				Color tempColor = new Color (population [index2].color.r, population [index2].color.g, population [index2].color.b);

				tempColor = mutatedColor (tempColor);

				child1.SetColor (tempColor);

				tempColor = new Color (population [index1].color.r, population [index1].color.g, population [index1].color.b);

				tempColor = mutatedColor (tempColor);

				child2.SetColor (tempColor);

				Vector3 tempSize = new Vector3 (population [index2].transform.localScale.x, population [index2].transform.localScale.y, population [index2].transform.localScale.z);

				tempSize = mutatedSize (tempSize);

				child1.SetSize (tempSize);

				tempSize = new Vector3 (population [index1].transform.localScale.x, population [index1].transform.localScale.y, population [index1].transform.localScale.z);

				tempSize = mutatedSize (tempSize);

				child2.SetSize (tempSize);
			}
			else if (howGenesAreSplit <= 0.64f) 
			{
				Color tempColor = new Color (population [index1].color.r, population [index2].color.g, population [index2].color.b);

				tempColor = mutatedColor (tempColor);

				child1.SetColor (tempColor);

				tempColor = new Color (population [index2].color.r, population [index1].color.g, population [index1].color.b);

				tempColor = mutatedColor (tempColor);

				child2.SetColor (tempColor);

				Vector3 tempSize = new Vector3 (population [index1].transform.localScale.x, population [index1].transform.localScale.y, population [index2].transform.localScale.z);

				tempSize = mutatedSize (tempSize);

				child1.SetSize (tempSize);

				tempSize = new Vector3 (population [index2].transform.localScale.x, population [index1].transform.localScale.y, population [index1].transform.localScale.z);

				tempSize = mutatedSize (tempSize);

				child2.SetSize (tempSize);
			}
			else if (howGenesAreSplit <= 0.8f) 
			{
				Color tempColor = new Color (population [index1].color.r, population [index2].color.g, population [index1].color.b);

				tempColor = mutatedColor (tempColor);

				child1.SetColor (tempColor);

				tempColor = new Color (population [index2].color.r, population [index1].color.g, population [index2].color.b);

				tempColor = mutatedColor (tempColor);

				child2.SetColor (tempColor);

				Vector3 tempSize = new Vector3 (population [index1].transform.localScale.x, population [index2].transform.localScale.y, population [index1].transform.localScale.z);

				tempSize = mutatedSize (tempSize);

				child1.SetSize (tempSize);

				tempSize = new Vector3 (population [index2].transform.localScale.x, population [index1].transform.localScale.y, population [index2].transform.localScale.z);

				tempSize = mutatedSize (tempSize);

				child2.SetSize (tempSize);
			}
			else
			{
				Color tempColor = new Color (population [index2].color.r, population [index1].color.g, population [index2].color.b);

				tempColor = mutatedColor (tempColor);

				child1.SetColor (tempColor);

				tempColor = new Color (population [index1].color.r, population [index2].color.g, population [index1].color.b);

				tempColor = mutatedColor (tempColor);

				child2.SetColor (tempColor);

				Vector3 tempSize = new Vector3 (population [index2].transform.localScale.x, population [index1].transform.localScale.y, population [index2].transform.localScale.z);

				tempSize = mutatedSize (tempSize);

				child1.SetSize (tempSize);

				tempSize = new Vector3 (population [index1].transform.localScale.x, population [index2].transform.localScale.y, population [index1].transform.localScale.z);

				tempSize = mutatedSize (tempSize);

				child2.SetSize (tempSize);
			}

			float tempTemp = Random.Range (population [index1].temperature, population [index2].temperature);

			tempTemp = mutatedTemperature (tempTemp);

			child1.SetTemp (tempTemp);

			tempTemp = Random.Range (population [index1].temperature, population [index2].temperature);

			tempTemp = mutatedTemperature (tempTemp);

			child2.SetTemp (tempTemp);
		}
		population.AddRange (tempList);
	}

	IEnumerator EvaluationLoop()
	{
		while (true) 
		{
			yield return new WaitForSeconds (evolveTime);
			EvaluatePopulation ();
			Generation++;
			generationText.text = "Generation " + Generation;
			tempText.text = string.Format ("{00:00.00}", eScript.temperature);
		}
	}

	public Color mutatedColor(Color bunnyColor)
	{
		float rateOfMutation = Random.Range (0f, 1f);

		Vector3 mutatedColor = new Vector3 (bunnyColor.r, bunnyColor.g, bunnyColor.b);

		for (int i = 0; i < 3; i++) 
		{
			if (Random.Range (0f, 1f) <= rateOfMutation) 
			{
				mutatedColor [i] = Random.Range (0f, 1f);
			}
		}

		return new Color (mutatedColor.x, mutatedColor.y, mutatedColor.z);
	}

	public Vector3 mutatedSize(Vector3 bunnySize)
	{
		float rateOfMutation = Random.Range (0f, 1f);

		Vector3 mutatedSize = bunnySize;

		for (int i = 0; i < 3; i++) 
		{
			if (Random.Range (0f, 1f) <= rateOfMutation) 
			{
				mutatedSize [i] = Random.Range (minSize, maxSize);
			}
		}

		return mutatedSize;
	}

	public float mutatedTemperature(float bunnyTemp)
	{
		float rateOfMutation = Random.Range (0f, 1f);

		float mutatedTemp = bunnyTemp;

		if (Random.Range (0f, 1f) <= rateOfMutation) 
		{
			mutatedTemp = Random.Range (minTemperature, maxTemperature);
		}

		return mutatedTemp;
	}
}
