using System.Collections.Generic;
using UnityEngine;

public class Souls : ScriptableObject
{
    public string name;
    public List<string> IdleReplices;
	public List<string> HoverReplices;
	public List<string> DragReplices;
	public List<string> PutInReplices;
	public List<string> InBoilReplices;
	public string bonusDescription;
	public bool SoulTalking = false;

    private int IdleReplicesIndex = 0;
	private int HoverReplicesIndex = 0;
    private int DragReplicesIndex = 0;
    private int PutInReplicesIndex = 0;

    public virtual void Bonus(BoilerManager boilerManager)
    {

    }
    public virtual void RemoveBonus(BoilerManager boilerManager)
    {

    }
	public void Reset()
	{
		IdleReplicesIndex = 0;
		HoverReplicesIndex = 0;
		DragReplicesIndex = 0;
		PutInReplicesIndex = 0;
	}

	public string GetIdleReplices()
	{
		if (IdleReplices == null || IdleReplices.Count < 1)
		{
			return "EmptyList";
		}

		if (IdleReplicesIndex >= IdleReplices.Count)
		{
			IdleReplicesIndex = 0;
		}

		return IdleReplices[IdleReplicesIndex++];
	}

	public string GetHoverReplices()
	{
		if (HoverReplices == null || HoverReplices.Count < 1)
		{
			return "EmptyList";
		}

		if (HoverReplicesIndex >= HoverReplices.Count)
		{
			HoverReplicesIndex = 0;
		}

		return HoverReplices[HoverReplicesIndex++];
	}

	public string GetDragReplices()
	{
		if (DragReplices == null || DragReplices.Count < 1)
		{
			return "EmptyList";
		}

		if (DragReplicesIndex >= DragReplices.Count)
		{
			DragReplicesIndex = 0;
		}

		return DragReplices[DragReplicesIndex++];
	}

	public string GetPutInReplices()
	{
		if (PutInReplices == null || PutInReplices.Count < 1)
		{
			return "EmptyList";
		}

		if (PutInReplicesIndex >= PutInReplices.Count)
		{
			PutInReplicesIndex = 0;
		}

		return PutInReplices[PutInReplicesIndex++];
	}


}