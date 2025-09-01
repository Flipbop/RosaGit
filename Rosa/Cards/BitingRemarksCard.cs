using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Flipbop.Rosa;

internal sealed class BitingRemarksCard : Card, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Cards.RegisterCard(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.RosaDeck.Deck,
				rarity = ModEntry.GetCardRarity(MethodBase.GetCurrentMethod()!.DeclaringType!),
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/BitingRemarks.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "BitingRemarks", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "FFFFFF",
			cost = upgrade == Upgrade.B? 1: 2,
			infinite = upgrade == Upgrade.B,
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.B => [
				new AStatus { targetPlayer = false, status = ModEntry.Instance.FrazzleStatus.Status, statusAmount = 1},
			],
			Upgrade.A => [
				new AStatus { targetPlayer = false, status = ModEntry.Instance.FrazzleStatus.Status, statusAmount = 3},
			],
			_ => [
				new AStatus { targetPlayer = false, status = ModEntry.Instance.FrazzleStatus.Status, statusAmount = 2},
			],
		};
}
