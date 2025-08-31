using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class ReroutePowerCard : Card, IRegisterable
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
			Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/ReroutePower.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "ReroutePower", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "996699",
			cost = upgrade == Upgrade.A ? 0 : 1,
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.B => [
				ModEntry.Instance.KokoroApi.ActionCosts.MakeCostAction(ModEntry.Instance.KokoroApi.ActionCosts.MakeResourceCost(new ImpairedCost(), 2), new AStatus{targetPlayer = true, status = Status.tempShield, statusAmount = 6}).AsCardAction,
			],
			_ => [
				ModEntry.Instance.KokoroApi.ActionCosts.MakeCostAction(ModEntry.Instance.KokoroApi.ActionCosts.MakeResourceCost(new ImpairedCost(), 1), new AStatus{targetPlayer = true, status = Status.tempShield, statusAmount = 3}).AsCardAction,
			]
		};
}
