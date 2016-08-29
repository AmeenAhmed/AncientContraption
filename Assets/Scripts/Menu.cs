using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	public GameObject text;
	int direction = 1;
	float scale = 1;
	float frameCount = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		scale += 0.5f * Time.deltaTime * direction;
		if (scale > 1.1) {
			direction = -1;
		}
		if (scale < 0.9) {
			direction = 1;
		}
		text.GetComponent<SpriteRenderer>().transform.localScale = new Vector3 (scale, scale, 1f);

		if (Input.anyKey) {
			SceneManager.LoadScene ("Scenes/Level-1");
		}
	}
}
