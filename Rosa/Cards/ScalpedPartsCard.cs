using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class ScalpedPartsCard : Card, IRegisterable
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
			Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/ScalpedParts.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "ScalpedParts", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "996699",
			cost = 2,
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.B => [
				new ADrawCard { count = 3},
				new ImprovedCannonCard.AUpgradeHint{hand = true},
				new AStatus { targetPlayer = true, status = Status.tempShield, statusAmount = 2*c.hand.Count(card => card.upgrade != Upgrade.None), xHint = 2},
				new AImpairHand()
			],
			Upgrade.A => [
				new ImprovedCannonCard.AUpgradeHint{hand = true},
				new AStatus { targetPlayer = true, status = Status.tempShield, statusAmount = 3*c.hand.Count(card => card.upgrade != Upgrade.None), xHint = 3},
				new AImpairHand()
			],
			_ => [
				new ImprovedCannonCard.AUpgradeHint{hand = true},
				new AStatus { targetPlayer = true, status = Status.tempShield, statusAmount = 2*c.hand.Count(card => card.upgrade != Upgrade.None), xHint = 2},
				new AImpairHand()
			],
		};
}
