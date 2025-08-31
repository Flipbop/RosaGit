using FSPRO;
using Nickel;
using System.Collections.Generic;
using System.Linq;


namespace Flipbop.Cleo;

public sealed class APermaFixB : DynamicWidthCardAction
{
	public required int Amount;

	public override void Begin(G g, State s, Combat c)
	{
		base.Begin(g, s, c);
		int index = c.hand.Count -1;
		int amountTemp = Amount;
		while (index >= 0 && Amount > 0)
		{
			if (c.hand[index].upgrade == Upgrade.None && c.hand[index].IsUpgradable() && c.hand[index].GetMeta().deck != Deck.trash)
			{
				c.hand[index].upgrade = Upgrade.B;
				Amount--;
				Audio.Play(Event.CardHandling);
			}
			index--;
		}
	}
}
