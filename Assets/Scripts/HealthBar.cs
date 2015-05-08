using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour {

	private Image healthBar;

	void Start() {
		healthBar = GetComponent<Image> ();
	}

	public float SubtractHealth (float amount) {
		healthBar.fillAmount -= amount;
		return healthBar.fillAmount;
	}
}
