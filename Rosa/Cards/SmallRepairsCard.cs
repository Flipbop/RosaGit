using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class SmallRepairsCard : Card, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Cards.RegisterCard(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.CleoDeck.Deck,
				rarity = ModEntry.GetCardRarity(MethodBase.GetCurrentMethod()!.DeclaringType!),
				upgradesTo = [Upgrade.A, Upgrade.B],
				dontOffer = true,
			},
			Art = ModEntry.Instance.TurtleShotSprite.Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "SmallRepairs", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "996699",
			cost = 1,
			temporary = true,
			exhaust = upgrade != Upgrade.B,
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=>
		[
			new AHeal { canRunAfterKill = true, targetPlayer = true, healAmount = upgrade == Upgrade.A ? 3 : 2 },
		];
};
