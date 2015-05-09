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

		if (healthBar.fillAmount <= 0.5f && healthBar.fillAmount > 0.25f) {
			healthBar.color = Color.yellow;
		} else if (healthBar.fillAmount <= 0.25f) {
			healthBar.color = Color.red;
		}

		return healthBar.fillAmount;
	}
}
