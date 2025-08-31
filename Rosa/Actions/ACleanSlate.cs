using FSPRO;
using Nickel;
using System.Collections.Generic;


namespace Flipbop.Cleo;

public sealed class ACleanSlate : DynamicWidthCardAction
{
	public override void Begin(G g, State s, Combat c)
	{
		base.Begin(g, s, c);
		int index = c.hand.Count -1;
		while (index >= 0)
		{
			Card temp = c.hand[index];
			if (temp.upgrade == Upgrade.None)
			{
				temp.ExhaustFX();
				Audio.Play(Event.CardHandling);
				c.hand.Remove(temp);
				c.SendCardToExhaust(s, temp);
			} 
			index--;
		}
	}
}
