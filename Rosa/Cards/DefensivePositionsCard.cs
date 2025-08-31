using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class DefensivePositionsCard : Card, IRegisterable
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
			Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/DefensivePositions.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "DefensivePositions", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "996699",
			cost = upgrade == Upgrade.B? 3 : 2,
			exhaust = upgrade == Upgrade.B
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.A => [
				new AStatus {targetPlayer = true, status = Status.tempShield, statusAmount = 4},
				ModEntry.Instance.KokoroApi.ActionCosts.MakeCostAction(ModEntry.Instance.KokoroApi.ActionCosts.MakeResourceCost(new ImpairedCost(), 2), new AStatus{targetPlayer = true, status = Status.tempPayback, statusAmount = 1}).AsCardAction,
			],
			Upgrade.B => [
				ModEntry.Instance.KokoroApi.ActionCosts.MakeCostAction(ModEntry.Instance.KokoroApi.ActionCosts.MakeResourceCost(new ImpairedCost(), 3), new AStatus{targetPlayer = true, status = Status.payback, statusAmount = 1}).AsCardAction,
			],
			_ => [
				new AStatus {targetPlayer = true, status = Status.tempShield, statusAmount = 2},
				ModEntry.Instance.KokoroApi.ActionCosts.MakeCostAction(ModEntry.Instance.KokoroApi.ActionCosts.MakeResourceCost(new ImpairedCost(), 2), new AStatus{targetPlayer = true, status = Status.tempPayback, statusAmount = 1}).AsCardAction,
			]
		};
}
