using FSPRO;
using Nickel;
using System.Collections.Generic;


namespace Flipbop.Cleo;

public sealed class APermaFix : DynamicWidthCardAction
{
	public required int Amount;

	public override void Begin(G g, State s, Combat c)
	{
		base.Begin(g, s, c);
		int index = c.hand.Count -1;
		while (index >= 0 && Amount > 0)
		{
			if (c.hand[index].upgrade == Upgrade.None && c.hand[index].IsUpgradable() && c.hand[index].GetMeta().deck != Deck.trash)
			{
				c.hand[index].upgrade = Upgrade.A;
				Amount--;
				Audio.Play(Event.CardHandling);
			}
			index--;
		}
	}
}
