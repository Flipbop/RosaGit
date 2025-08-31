using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class PowerSurgeCard : Card, IRegisterable
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
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/PowerSurge.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "PowerSurge", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "996699",
			cost = 3,
			exhaust = true
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.A => [
				new ADrawCard {count = 2},
				new AImproveAHand(),
				new ADiscard {count = 3}
			],
			Upgrade.B => [
				new ADrawCard {count = 1},
				new AImproveBHand(),
				new ADiscountHand { Amount = -1},
				new ADiscard(),
			],
			_ => [
				new ADrawCard {count = 2},
				new AImproveAHand(),
				new ADiscard(),
			]
		};
}
