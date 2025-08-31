using FSPRO;
using Nickel;
using System.Collections.Generic;


namespace Flipbop.Cleo;

public sealed class ADrawUpgrade : DynamicWidthCardAction
{
	public int Amount;

	public override void Begin(G g, State s, Combat c)
	{
		base.Begin(g, s, c);
		int index = s.deck.Count - 1;
		while (index >= 0 && Amount > 0)
		{
			if (s.deck[index].upgrade != Upgrade.None)
			{
				c.DrawCardIdx(s, index).waitBeforeMoving = (double) Amount * 0.09;
				Amount--;
			}
			index--;
		}
	}
}
