using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour {

	public int maxHealth;
	private Image healthBar;

	void Start() {
		healthBar = GetComponent<Image> ();
	}

	void subtractHealth (float amount) {
		healthBar.fillAmount -= amount;
	}
}
