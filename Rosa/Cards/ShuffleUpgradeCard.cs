using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class ShuffleUpgradeCard : Card, IRegisterable
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
			Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/ShuffleUpgrade.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "ShuffleUpgrade", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "996699",
			cost = 1,
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.B => [
				new AImproveB { Amount = 1 },
				new AShuffleHand(),
				new AImproveB { Amount = 1 },
			],
			Upgrade.A => [
				new AShuffleHand(),
				new AImproveA { Amount = 2 },
				new AStatus { targetPlayer = true, status = Status.tempShield, statusAmount = 2 },
			],
			_ => [
				new AShuffleHand(),
				new AImproveA { Amount = 2 },
			]
		};
}
