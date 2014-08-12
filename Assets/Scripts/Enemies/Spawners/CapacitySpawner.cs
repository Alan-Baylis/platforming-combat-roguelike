using UnityEngine;
using System.Collections.Generic;

public class CapacitySpawner : Spawner
{
	public List<GameObject> spawned;
	public float capacity;

	void Start()
	{
		spawned = new List<GameObject>();
		base.Start();
	}

	override protected void Spawn(Vector2 pos)
	{
		//clean up the list
		for(int i = 0; i < spawned.Count; i++) {
			if(spawned[i] == null) {
				spawned.RemoveAt(i);
				i--;
			}
		}
		if(spawned.Count < capacity) {
			GameObject enemy = Create(pos);
			if(enemy)
				AddChild(enemy);
		}
	}

	void AddChild(GameObject child)
	{
		spawned.Add(child);
	}



}

