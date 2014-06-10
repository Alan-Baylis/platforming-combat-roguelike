using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilitiesController : MonoBehaviour
{

	public float[] abInput;
	public Ability[] activeAbilities;
	public List<Ability> allAbilities;
	public Transform channeller;
	bool inputAllowed = true;

	void Awake()
	{
		abInput = new float[4];

	}

	void Start()
	{
		if(allAbilities == null)
			allAbilities = new List<Ability>();
		for(int i = 0; i < activeAbilities.Length; i++)
			if(activeAbilities[i] != null)
				allAbilities.Add(activeAbilities[i]);
	}

	void Update()
	{
		if(inputAllowed) {
			abInput[0] = Input.GetAxisRaw("A1");
			abInput[1] = Input.GetAxisRaw("A2");
			abInput[2] = Input.GetAxisRaw("A3");
			abInput[3] = Input.GetAxisRaw("A4");
		}


		for(int i = 0; i < abInput.Length; i++) {
			if(abInput[i] > 0 && activeAbilities[i] != null) {
				activeAbilities[i].triggerActive(transform);
			}
		}
	}

	public void AddAbility(Ability ab, int slot)
	{
		allAbilities.Add(ab);
		if(slot >= 0)
			activeAbilities[slot] = ab;
	}

	public void UpgradeAbility(Ability upg, int slot)
	{
		allAbilities.Add(upg);
		if(slot >= 0 && activeAbilities[slot] != null)
			activeAbilities[slot].upgradeAbility(upg);
	}

	public void SetAbility(int abIndex, int activeIndex)
	{
		activeAbilities[activeIndex] = allAbilities[abIndex];
	}

	public bool IsActive(Ability ab)
	{
		foreach(Ability active in activeAbilities) {
			if(active != null && ab.abilityName == active.abilityName)
				return true;
		}
		return false;
	}

	public void ToggleInput()
	{
		inputAllowed = !inputAllowed;
		ClearInput();
	}

	void ClearInput()
	{
		for(int i = 0; i < abInput.Length; i++) {
			abInput[i] = 0;
		}
	}

}

