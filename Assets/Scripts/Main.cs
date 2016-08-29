using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour {
	string mode = "build";
	string cursorMode = "move";

	public GameObject spr_stick;
	public GameObject spr_stone;
	public GameObject spr_nail;
	public GameObject spr_hinge;

	public GameObject obj_stick;
	public GameObject obj_stone;
	public GameObject obj_nail;
	public GameObject obj_hinge;


	public Texture2D cursor;
	public Texture2D rotate_cursor;
	public Sprite play_image;
	public Sprite pause_image;


	public ArrayList objects;
	public ArrayList gameObjects;

	public GameObject selectedObject = null;
	public Vector3 offset;

	public Button play;

	public string clickToPlaceObject = null;

	public GameObject target;
	public GameObject src;
	public Vector3 srcObjectPosition;


	public Image startMessage;
	public Image endMessage;

	public Image extraMessage1;
	public Image extraMessage2;

	public Image[] tuts;

	public AudioSource clickSound;
	public AudioSource playSound;
	public AudioSource winSound;

	// Use this for initialization
	void Start () {
		objects = new ArrayList ();
		gameObjects = new ArrayList ();
		srcObjectPosition = src.transform.position;

		Time.timeScale = 0;
		Cursor.SetCursor (cursor, new Vector2(0,0),CursorMode.Auto);
		endMessage.gameObject.SetActive (false);
		if (extraMessage1 != null) {
			extraMessage2.gameObject.SetActive (false);
			startMessage.gameObject.SetActive (false);
		}
	}

	public void tutorialNext (int num) {
		Debug.Log (num);
		Destroy (tuts [num].transform.GetChild (0).gameObject);
		Destroy (tuts [num]);
	}

	public void destroyStartMessage () {
		Destroy (startMessage.transform.GetChild (0).gameObject);
		Destroy (startMessage);
	}

	public void destroyExtraMessage1 () {
		Destroy (extraMessage1.transform.GetChild (0).gameObject);
		Destroy (extraMessage1);	
		extraMessage2.gameObject.SetActive (true);
		startMessage.gameObject.SetActive (true);
	}

	public void destroyExtraMessage2 () {
		Destroy (extraMessage2.transform.GetChild (0).gameObject);
		Destroy (extraMessage2);
	}

	public void nextLevel (int num) {
		SceneManager.LoadScene ("Scenes/Level-" + num);
	}

	GameObject findObjectAtMousePosition (out Vector3 offset) {
		RaycastHit2D hit;
//		Debug.Log ("Raycasting");
		hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint(Input.mousePosition), -Vector2.zero);
		if (hit.collider != null) {
//			Debug.Log (hit.collider.name);
			offset = new Vector3 (hit.transform.position.x - hit.point.x, hit.transform.position.y - hit.point.y, 0);
			if (hit.collider.gameObject.GetComponent<ObjectData> ()) {
				return hit.collider.gameObject;
			} else {
				return null;
			}

		}
		offset = Vector3.zero;
		return null;
	}

	GameObject[] findAllObjectsAtMousePoistion (out Vector3[] offsets) {
		RaycastHit2D[] hits;
		hits = Physics2D.RaycastAll (Camera.main.ScreenToWorldPoint (Input.mousePosition), -Vector2.zero);

		if (hits.Length > 0) {
			offsets = new Vector3[hits.Length];
			GameObject[] objs = new GameObject[hits.Length];
			for (int i = 0; i < hits.Length; i++) {
				RaycastHit2D hit = hits [i];
				objs [i] = hits [i].collider.gameObject;
				Vector3 offset = new Vector3 (hit.point.x - hit.transform.position.x,  hit.point.y - hit.transform.position.y, 0);
				offsets [i] = offset;
			}
			return objs;
		}
		offsets = null;
		return null;
	}
	
	// Update is called once per frame
	void Update () {
		if (mode == "build") {
			if (Input.GetMouseButtonDown (0)) {
				if (cursorMode == "place") {
					Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);

					if (clickToPlaceObject == "nail") {
						Vector3[] offsets = null;
						GameObject[] objs = findAllObjectsAtMousePoistion (out offsets);

						if (objs.Length > 0) {
							GameObject obj = Instantiate (spr_nail);
							obj.transform.position = new Vector3 (mousePos.x, mousePos.y, 0f);
							obj.GetComponent<ObjectData> ().connectedTo = objs;
							objects.Add (obj);

//							Debug.Log (obj.GetComponent<ObjectData> ().connectedTo [0]);
							cursorMode = "move";
							clickToPlaceObject = null;
						}
					} else if (clickToPlaceObject == "hinge") {
						Vector3 []offsets = null;
						GameObject[] objs = findAllObjectsAtMousePoistion (out offsets);

						if (objs.Length > 0) {
							GameObject obj = Instantiate (spr_hinge);
							obj.transform.position = new Vector3 (mousePos.x, mousePos.y, 0f);
							obj.GetComponent<ObjectData> ().connectedTo = objs;
							obj.GetComponent<ObjectData> ().atOffsets = offsets;
							objects.Add (obj);

//							Debug.Log (obj.GetComponent<ObjectData> ().connectedTo [0]);
							cursorMode = "move";
							clickToPlaceObject = null;
						}
					}
				} else {
					selectedObject = findObjectAtMousePosition (out offset);
				}
			}

			if (selectedObject) {
				if (cursorMode == "move") {
					Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);

					selectedObject.transform.position = new Vector3 (mousePos.x + offset.x, mousePos.y + offset.y, 0f);	
				} else if (cursorMode == "rotate") {
					Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
//					float angle = Vector3.Angle (selectedObject.transform.position, mousePos);
					float angle = Mathf.Atan2(selectedObject.transform.position.y - mousePos.y, selectedObject.transform.position.x - mousePos.x) * Mathf.Rad2Deg;
					selectedObject.transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, angle));
//					selectedObject.transform.rotation.eulerAngles.z = Mathf.Atan2((mousePos.y - selectedObject.transform.position.y), (mousePos.x - selectedObject.transform.position.x))*Mathf.Rad2Deg;
				}

			}

			if (Input.GetMouseButtonUp (0)) {
				selectedObject = null;
			}
		}
	}

	GameObject createObjectOfType (string type, GameObject src) {
		if (type == "stick") {
			return Instantiate (obj_stick) as GameObject;
		} else if (type == "stone") {
			return Instantiate (obj_stone) as GameObject;
		} else if (type == "nail") {
			GameObject go = Instantiate (obj_nail) as GameObject;

			go.GetComponent<FixedJoint2D> ().connectedBody = src.GetComponent<ObjectData> ().connectedTo [0].GetComponent<ObjectData> ().go.GetComponent<Rigidbody2D> ();

			if (src.GetComponent<ObjectData> ().connectedTo.Length > 0) {
				
				for (int i = 1; i < src.GetComponent<ObjectData> ().connectedTo.Length; i++) {
					GameObject obj = src.GetComponent<ObjectData> ().connectedTo [i - 1].GetComponent<ObjectData> ().go;
					GameObject obj1 = src.GetComponent<ObjectData> ().connectedTo [i].GetComponent<ObjectData> ().go;
					Debug.Log (obj);
//					obj1.GetComponent<Rigidbody2D> ().isKinematic = true;
//					obj.GetComponent<Rigidbody2D> ().isKinematic = true;
					if (!obj1.GetComponent<FixedJoint2D> ()) {
						obj1.AddComponent<FixedJoint2D> ();
						obj1.GetComponent<FixedJoint2D> ().connectedBody = obj.GetComponent<Rigidbody2D> ();
					} else {
						obj.AddComponent<FixedJoint2D> ();
						obj.GetComponent<FixedJoint2D> ().connectedBody = obj1.GetComponent<Rigidbody2D>();
					}

//					obj1.GetComponent<Rigidbody2D> ().isKinematic = false;
//					obj.GetComponent<Rigidbody2D> ().isKinematic = false;

				}
			}
			return go;
		} else if (type == "hinge") {
			GameObject go = Instantiate (obj_hinge) as GameObject;

			go.GetComponent<HingeJoint2D> ().connectedBody = src.GetComponent<ObjectData> ().connectedTo [0].GetComponent<ObjectData> ().go.GetComponent<Rigidbody2D> ();

			if (src.GetComponent<ObjectData> ().connectedTo.Length > 0) {

				for (int i = 1; i < src.GetComponent<ObjectData> ().connectedTo.Length; i++) {
					GameObject obj = src.GetComponent<ObjectData> ().connectedTo [i - 1].GetComponent<ObjectData> ().go;
					GameObject obj1 = src.GetComponent<ObjectData> ().connectedTo [i].GetComponent<ObjectData> ().go;
					Debug.Log (obj);
					//					obj1.GetComponent<Rigidbody2D> ().isKinematic = true;
					//					obj.GetComponent<Rigidbody2D> ().isKinematic = true;
					if (!obj1.GetComponent<HingeJoint2D> ()) {
						obj1.AddComponent<HingeJoint2D> ();
						obj1.GetComponent<HingeJoint2D> ().connectedBody = obj.GetComponent<Rigidbody2D> ();
						obj1.GetComponent<HingeJoint2D> ().anchor = src.GetComponent<ObjectData>().atOffsets [i];
					} else {
						obj.AddComponent<HingeJoint2D> ();
						obj.GetComponent<HingeJoint2D> ().connectedBody = obj1.GetComponent<Rigidbody2D>();
						obj.GetComponent<HingeJoint2D> ().anchor = src.GetComponent<ObjectData>().atOffsets [i-1];
					}

					//					obj1.GetComponent<Rigidbody2D> ().isKinematic = false;
					//					obj.GetComponent<Rigidbody2D> ().isKinematic = false;

				}
			}
			return go;
		}
		return null;
	}

	void changeMode (string mode) {
		if (mode == "play") {
			
			foreach (GameObject obj in objects) {
				obj.GetComponent<SpriteRenderer> ().enabled = false;
				string type = obj.GetComponent<ObjectData> ().type;
				GameObject go = createObjectOfType (type, obj);
				obj.GetComponent<ObjectData> ().go = go;
				go.transform.position = new Vector3 (obj.transform.position.x, obj.transform.position.y, 0f);
				go.transform.rotation = new Quaternion (obj.transform.rotation.x, obj.transform.rotation.y,
														obj.transform.rotation.z, obj.transform.rotation.w);
				gameObjects.Add (go);
			}
			Time.timeScale = 1;
		} else {
			Time.timeScale = 0;
			src.transform.position = new Vector3 (srcObjectPosition.x, srcObjectPosition.y, srcObjectPosition.z);
			src.GetComponent<Rigidbody2D> ().velocity = Vector3.zero;
			src.GetComponent<Rigidbody2D> ().angularVelocity = 0f;
			foreach (GameObject obj in gameObjects) {
				Destroy (obj);
			}
			foreach (GameObject obj in objects) {
				obj.GetComponent<SpriteRenderer> ().enabled = true;
			}
			gameObjects.Clear ();
		}
	}

	public void toggleMode () {
		if (mode == "build") {
			mode = "play";
			play.image.sprite = pause_image;
		} else {
			mode = "build";
			play.image.sprite = play_image;
		}

		changeMode (mode);
	}

	public void createObject (string type) {
		GameObject go = null;
		if (type == "stick") {
			go = Instantiate (spr_stick) as GameObject;
		} else if (type == "stone") {
			go = Instantiate (spr_stone) as GameObject;
		} 

		if (go) {
			objects.Add (go);
		}
	}

	public void createObjectOnClick (string type) {
		clickToPlaceObject = type;
		changeCursorMode ("place");
	}

	public void changeCursorMode (string mode) {
		cursorMode = mode;
		if (mode == "move") {
			Cursor.SetCursor (cursor, new Vector2 (0, 0), CursorMode.Auto);
		} else if (mode == "rotate") {
			Cursor.SetCursor (rotate_cursor, new Vector2 (0, 0), CursorMode.Auto);
		} else if (mode == "place") {
			
		}
	}

	public void targetHit () {
		endMessage.gameObject.SetActive (true);
		Debug.Log ("Done!!");
		winSound.Play ();
	}

	public void playClickSound () {
		clickSound.Play ();
	}

	public void playPlaySound () {
		playSound.Play ();
	}

	public void showFinalScreen () {
		SceneManager.LoadScene ("Scenes/Final");
	}
}
