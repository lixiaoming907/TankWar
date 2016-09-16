using UnityEngine;
using System.Collections;

public class EffectLifeTime : MonoBehaviour
{

    public float lifeTime = 1f;

	// Use this for initialization
	void Start () {
	    Destroy(this.gameObject,lifeTime);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
