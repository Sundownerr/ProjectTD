using System.Collections;
using System.Collections.Generic;
using Game.Data;
using UnityEngine;

public interface IAffectable
{
	void Add(Effect effect);
	void Remove(Effect effect);
	int CountOf(Effect effect);
}

public interface IEffect
{
	
}

public interface ITowerSystem
{
	void Set();
}